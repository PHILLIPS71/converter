using System.IO.Abstractions;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;

public sealed class FileSystemFile : FileSystemEntry
{
    private FileSystemFile()
    {
    }

    internal FileSystemFile(IFileInfo entry, FileSystemDirectory? parent = null)
        : base(entry)
    {
    }
}