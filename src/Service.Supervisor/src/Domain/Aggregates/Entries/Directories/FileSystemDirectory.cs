using ErrorOr;
using System.IO.Abstractions;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

public sealed class FileSystemDirectory : FileSystemEntry
{
    private FileSystemDirectory()
    {
    }

    internal FileSystemDirectory(IDirectoryInfo entry, FileSystemDirectory? parent = null)
        : base(entry, parent)
    {
    }

    public List<FileSystemEntry> Entries { get; private set; } = [];

    public ErrorOr<bool> TryScan(IFileSystem fs)
    {
        var paths = new List<string> { PathInfo.FullName };

        try
        {
            var directory = fs.DirectoryInfo.New(PathInfo.FullName);

            foreach (var info in directory.GetFileSystemInfos())
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

                paths.Add(info.FullName);
            }
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
        Entries.RemoveAll(x => paths.TrueForAll(path => path != x.PathInfo.FullName));
        return true;
    }
}