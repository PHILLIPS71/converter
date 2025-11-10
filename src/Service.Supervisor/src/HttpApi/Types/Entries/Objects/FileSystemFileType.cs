using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Interfaces;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Objects;

[ObjectType<FileSystemFile>]
public static partial class FileSystemFileType
{
    static partial void Configure(IObjectTypeDescriptor<FileSystemFile> descriptor)
    {
        descriptor.Implements<FileSystemEntryType>();

        descriptor
            .Field(f => f.Id);

        descriptor
            .Field(f => f.PathInfo);

        descriptor
            .Field(f => f.Size);

        descriptor
            .Field(f => f.VideoStreams);

        descriptor
            .Field(f => f.AudioStreams);

        descriptor
            .Field(f => f.SubtitleStreams);

        descriptor
            .Field(f => f.CreatedAt);

        descriptor
            .Field(f => f.UpdatedAt);
    }

    [NodeResolver]
    public static Task<FileSystemFile?> GetFileSystemFileByIdAsync(
        Id id,
        QueryContext<FileSystemFile> query,
        IFileSystemFileByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.With(query).LoadAsync(id, cancellation);
    }

    [DataLoader]
    internal static Task<Dictionary<Id, FileSystemFile>> GetFileSystemFileByIdAsync(
        IReadOnlyList<Id> keys,
        QueryContext<FileSystemFile> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Files
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query)
            .ToDictionaryAsync(x => x.Id, cancellation);
    }
}
