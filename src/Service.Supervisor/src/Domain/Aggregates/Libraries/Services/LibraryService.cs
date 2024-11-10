using System.IO.Abstractions;
using ErrorOr;
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
        var directory = _fs.DirectoryInfo.New(path);
        if (!directory.Exists)
            return Error.NotFound(description: $"directory at path '{path}' does not exist");

        var isNameUsed = await _libraries.ExistsAsync(x => x.Name == name, cancellation);
        if (isNameUsed)
            return Error.Conflict(description: $"library with name '{name.Value}' already exists");

        var isSlugUsed = await _libraries.ExistsAsync(x => x.Slug == slug, cancellation);
        if (isSlugUsed)
            return Error.Conflict(description: $"library with slug '{slug.Value}' already exists");

        var entry = await _directories.SingleOrDefaultAsync(x => x.PathInfo.FullName == path, cancellation);
        if (entry == null)
            entry = _directories.Create(new FileSystemDirectory(directory));

        var library = Library.Create(entry, name, slug);
        if (library.IsError)
            return library.Errors;

        return _libraries.Create(library.Value);
    }
}