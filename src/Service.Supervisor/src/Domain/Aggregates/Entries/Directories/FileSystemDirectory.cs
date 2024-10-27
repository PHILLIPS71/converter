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

    public ICollection<FileSystemEntry> Entries { get; private set; } = new List<FileSystemEntry>();
}