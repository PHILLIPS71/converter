using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;

public sealed class FileSystemFile : FileSystemEntry
{
    private FileSystemFile()
    {
    }

    internal FileSystemFile(PathInfo path, long size, FileSystemDirectory? parent = null)
        : base(path, size, parent)
    {
    }
}