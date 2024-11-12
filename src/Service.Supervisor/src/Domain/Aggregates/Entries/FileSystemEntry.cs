using ErrorOr;
using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Values;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;

public abstract class FileSystemEntry : AggregateRoot<Guid>, ITimestampableEntity
{
    protected FileSystemEntry()
    {
    }

    protected internal FileSystemEntry(IFileSystemInfo entry, long size, FileSystemDirectory? parent = null)
    {
        Id = NewId.NextSequentialGuid();
        Parent = parent;
        PathInfo = new PathInfo(entry);
        Size = size;
    }

    public static ErrorOr<FileSystemEntry> Create(IFileSystemInfo entry, FileSystemDirectory? parent = null)
    {
        return entry switch
        {
            IDirectoryInfo directory => new FileSystemDirectory(directory, parent),
            IFileInfo file => new FileSystemFile(file, parent),
            _ => Error.Unexpected()
        };
    }

    public FileSystemDirectory? Parent { get; protected set; }

    public PathInfo PathInfo { get; protected set; }

    public long Size { get; protected set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}