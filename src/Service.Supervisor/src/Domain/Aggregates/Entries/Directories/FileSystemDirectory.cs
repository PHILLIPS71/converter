using System.IO.Abstractions;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

public sealed class FileSystemDirectory : FileSystemEntry
{
    private FileSystemDirectory()
    {
    }

    public FileSystemDirectory(IDirectoryInfo entry)
        : base(entry)
    {
    }

    public ICollection<FileSystemEntry> Entries { get; private set; } = new List<FileSystemEntry>();
}