using ErrorOr;
using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Enumerations;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

public sealed class FileSystemDirectory : FileSystemEntry
{
    private FileSystemDirectory()
    {
    }

    internal FileSystemDirectory(PathInfo path, FileSystemDirectory? parent = null)
        : base(path, 0, parent)
    {
    }

    public List<FileSystemEntry> Entries { get; private set; } = [];

    public ErrorOr<bool> TryScan(IFileSystem fs)
    {
        var paths = new List<string> { PathInfo.FullName };
        var size = 0L;

        try
        {
            var directory = fs.DirectoryInfo.New(PathInfo.FullName);

            // collect all directories or files that have an extension that is deemed a video file
            var infos = directory.GetFileSystemInfos("*", SearchOption.TopDirectoryOnly)
                .Where(x => x is IDirectoryInfo || Enumeration.TryParse<VideoFileContainer>(container => string.Equals(container.Extension, x.Extension, StringComparison.InvariantCultureIgnoreCase), out _))
                .ToList();

            foreach (var info in infos)
            {
                // check if the entry already exists; otherwise, create and add it to the collection
                var entry = Entries.SingleOrDefault(x => x.PathInfo.FullName == info.FullName);
                if (entry == null)
                {
                    var result = Create(info, this);
                    if (result.IsError)
                        return result.Errors;

                    entry = result.Value;
                    Entries.Add(result.Value);
                }

                if (entry is FileSystemDirectory subdirectory)
                {
                    var result = subdirectory.TryScan(fs);
                    if (result.IsError)
                        return result.Errors;
                }

                size += entry.Size;
                paths.Add(info.FullName);
            }

            Size = size;
        }
        catch (DirectoryNotFoundException)
        {
            return Error.NotFound(description: $"the directory was not found at {PathInfo.FullName}");
        }
        catch (UnauthorizedAccessException)
        {
            return Error.Unauthorized(description: $"access was denied reading directory at {PathInfo.FullName}");
        }
        catch (Exception)
        {
            return Error.Unexpected(description: $"failure reading directory at {PathInfo.FullName}");
        }

        // remove entries that no longer exist in the file system
        Entries.RemoveAll(entry => paths.TrueForAll(path => path != entry.PathInfo.FullName));
        return true;
    }
}