using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.HttpApi.Endpoints.Entries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Directories;

[ObjectType<FileSystemDirectory>]
internal static partial class FileSystemDirectoryType
{
    static partial void Configure(IObjectTypeDescriptor<FileSystemDirectory> descriptor)
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
            .Field(f => f.ScannedAt);

        descriptor
            .Field(f => f.CreatedAt);

        descriptor
            .Field(f => f.UpdatedAt);
    }

    public static FileSystemDistribution GetDistribution(
        [Parent(nameof(FileSystemDirectory.PathInfo))]
        FileSystemDirectory directory)
        => new FileSystemDistribution(directory.PathInfo);

    [NodeResolver]
    public static async Task<FileSystemDirectory?> GetDirectoryByIdAsync(
        Id id,
        QueryContext<FileSystemDirectory> query,
        IDirectoryByIdDataLoader dataloader,
        CancellationToken cancellation)
        => await dataloader
            .With(query)
            .LoadAsync(id, cancellation);

    [UsePaging]
    [UseFiltering]
    public static async Task<Connection<FileSystemEntry>> GetEntriesAsync(
        [Parent(requires: nameof(FileSystemDirectory.ParentId))]
        FileSystemDirectory directory,
        PagingArguments paging,
        QueryContext<FileSystemEntry> query,
        IEntriesByDirectoryIdDataLoader dataloader,
        CancellationToken cancellation = default)
        => await dataloader
            .With(paging, query)
            .LoadAsync(directory.Id, cancellation)
            .ToConnectionAsync();

    [DataLoader]
    internal static async Task<Dictionary<Id, FileSystemDirectory>> GetDirectoryByIdAsync(
        IReadOnlyList<Id> keys,
        QueryContext<FileSystemDirectory> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
        => await database
            .Directories
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query, x => x.AddAscending(y => y.Id))
            .ToDictionaryAsync(x => x.Id, cancellation);

    [DataLoader]
    internal static async Task<Dictionary<Id, Page<FileSystemEntry>>> GetEntriesByDirectoryIdAsync(
        IReadOnlyList<Id> keys,
        PagingArguments paging,
        QueryContext<FileSystemEntry> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
        => await database
            .Entries
            .AsNoTracking()
            .Where(x => x.ParentId.HasValue && keys.Contains(x.ParentId.Value))
            .With(query, x => x.AddAscending(y => y.Id))
            .ToBatchPageAsync(x => x.ParentId!.Value, paging, cancellation);
}
