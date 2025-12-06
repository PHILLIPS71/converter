using System.IO.Abstractions;
using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;

public abstract class FileSystemEntry : AggregateRoot<Id>, ITimestampableEntity
{
    protected FileSystemEntry()
    {
    }

    private protected FileSystemEntry(PathInfo path, long size, FileSystemDirectory? parent = null)
    {
        Id = Id.NewId();
        Parent = parent;
        PathInfo = path;
        Size = size;
    }

    public static ErrorOr<FileSystemEntry> Create(IFileSystemInfo entry, FileSystemDirectory? parent = null)
    {
        var path = PathInfo.Create(entry);
        if (path.IsError)
            return path.Errors;

        return entry switch
        {
            IDirectoryInfo => new FileSystemDirectory(path.Value, parent),
            IFileInfo file => new FileSystemFile(path.Value, file.Length, parent),
            _ => Error.Unexpected()
        };
    }

    public Id? ParentId { get; protected set; }
    public FileSystemDirectory? Parent { get; protected set; }

    public PathInfo PathInfo { get; protected set; }

    public long Size { get; protected set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}
