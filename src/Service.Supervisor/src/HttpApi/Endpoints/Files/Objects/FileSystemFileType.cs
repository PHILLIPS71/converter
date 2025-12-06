using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.HttpApi.Endpoints.Entries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Files;

[ObjectType<FileSystemFile>]
internal static partial class FileSystemFileType
{
    static partial void Configure(IObjectTypeDescriptor<FileSystemFile> descriptor)
    {
        descriptor.Implements<FileSystemEntryType>();

        descriptor
            .Field(f => f.Id);

        descriptor
            .Field(f => f.Parent);

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
    public static async Task<FileSystemFile?> GetFileSystemFileByIdAsync(
        Id id,
        QueryContext<FileSystemFile> query,
        IFileSystemFileByIdDataLoader dataloader,
        CancellationToken cancellation)
        => await dataloader
            .With(query)
            .LoadAsync(id, cancellation);

    [DataLoader]
    internal static async Task<Dictionary<Id, FileSystemFile>> GetFileSystemFileByIdAsync(
        IReadOnlyList<Id> keys,
        QueryContext<FileSystemFile> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
        => await database
            .Files
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query)
            .ToDictionaryAsync(x => x.Id, cancellation);
}
