using System.IO.Abstractions;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;

public sealed class FileSystemFile : FileSystemEntry
{
    private FileSystemFile()
    {
    }

    public FileSystemFile(IFileInfo entry)
        : base(entry)
    {
        Size = entry.Length;
    }

    public long Size { get; private set; }
}