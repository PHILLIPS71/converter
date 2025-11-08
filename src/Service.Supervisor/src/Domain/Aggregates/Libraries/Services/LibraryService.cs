using System.IO.Abstractions;
using ErrorOr;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

internal sealed class LibraryService : ILibraryService
{
    private readonly IFileSystem _fs;
    private readonly ILibraryRepository _libraries;
    private readonly IDirectoryRepository _directories;

    public LibraryService(IFileSystem fs, ILibraryRepository libraries, IDirectoryRepository directories)
    {
        _fs = fs;
        _libraries = libraries;
        _directories = directories;
    }

    /// <inheritdoc cref="CreateAsync"/>
    public async Task<ErrorOr<Library>> CreateAsync(
        LibraryName name,
        LibrarySlug slug,
        string path,
        CancellationToken cancellation = default)
    {
        var isPathUsed = await _libraries.ExistsAsync(x => x.Directory.PathInfo.FullName == path, cancellation);
        if (isPathUsed)
            return Error.Conflict(description: $"a library with path '{path}' already exists");

        var isNameUsed = await _libraries.ExistsAsync(x => x.Name == name, cancellation);
        if (isNameUsed)
            return Error.Conflict(description: $"a library with name '{name.Value}' already exists");

        var isSlugUsed = await _libraries.ExistsAsync(x => x.Slug == slug, cancellation);
        if (isSlugUsed)
            return Error.Conflict(description: $"a library with slug '{slug.Value}' already exists");

        var directory = _fs.DirectoryInfo.New(path);
        if (!directory.Exists)
            return Error.NotFound(description: $"a directory at path '{path}' does not exist");

        var root = await _directories.SingleOrDefaultAsync(x => x.PathInfo.FullName == path, cancellation);
        if (root == null)
        {
            var entry = FileSystemEntry.Create(directory);
            if (entry.IsError)
                return entry.Errors;

            if (entry.Value is not FileSystemDirectory dir)
                return Error.Unexpected(description: $"a directory was expected to be created, but received a {entry.Value.GetType()}");

            root = _directories.Create(dir);
        }

        var library = Library.Create(root, name, slug);
        if (library.IsError)
            return library.Errors;

        return _libraries.Create(library.Value);
    }
}
