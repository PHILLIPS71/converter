using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Values;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;

public abstract class FileSystemEntry : AggregateRoot<Guid>, ITimestampableEntity
{
    protected FileSystemEntry()
    {
    }

    protected internal FileSystemEntry(IFileSystemInfo entry, FileSystemDirectory? parent = null)
    {
        Id = NewId.NextSequentialGuid();
        Parent = parent;
        PathInfo = new PathInfo(entry);
    }

    public FileSystemDirectory? Parent { get; protected set; }

    public PathInfo PathInfo { get; protected set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}