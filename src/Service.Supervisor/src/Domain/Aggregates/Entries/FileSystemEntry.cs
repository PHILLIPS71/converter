using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Values;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;

public abstract class FileSystemEntry : AggregateRoot<Guid>, ITimestampableEntity
{
    protected FileSystemEntry()
    {
    }

    protected FileSystemEntry(IFileSystemInfo entry)
    {
        Id = NewId.NextSequentialGuid();
        PathInfo = new PathInfo(entry);
    }

    public PathInfo PathInfo { get; protected set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}