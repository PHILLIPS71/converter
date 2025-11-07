using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Interfaces;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Objects;

[ObjectType<FileSystemDirectory>]
public static partial class FileSystemDirectoryType
{
    static partial void Configure(IObjectTypeDescriptor<FileSystemDirectory> descriptor)
    {
        descriptor.Implements<FileSystemEntryType>();

        descriptor
            .Field(f => f.Id)
            .ID();

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
        [Parent] FileSystemDirectory directory)
    {
        return new FileSystemDistribution(directory.PathInfo);
    }

    [NodeResolver]
    public static Task<FileSystemDirectory?> GetDirectoryByIdAsync(
        Guid id,
        QueryContext<FileSystemDirectory> query,
        IDirectoryByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.With(query).LoadAsync(id, cancellation);
    }

    [UsePaging]
    [UseFiltering]
    public static async Task<Connection<FileSystemEntry>> GetEntriesAsync(
        [Parent] FileSystemDirectory directory,
        PagingArguments paging,
        QueryContext<FileSystemEntry> query,
        IEntriesByDirectoryIdDataLoader dataloader,
        CancellationToken cancellation = default)
    {
        return await dataloader
            .With(paging, query)
            .LoadAsync(directory.Id, cancellation)
            .ToConnectionAsync();
    }

    [DataLoader]
    internal static Task<Dictionary<Guid, FileSystemDirectory>> GetDirectoryByIdAsync(
        IReadOnlyList<Guid> keys,
        QueryContext<FileSystemDirectory> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Directories
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query, x => x.AddAscending(y => y.Id))
            .ToDictionaryAsync(x => x.Id, cancellation);
    }

    [DataLoader]
    internal static ValueTask<Dictionary<Guid, Page<FileSystemEntry>>> GetEntriesByDirectoryIdAsync(
        IReadOnlyList<Guid> keys,
        PagingArguments paging,
        QueryContext<FileSystemEntry> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Entries
            .AsNoTracking()
            .Where(x => x.Parent != null && keys.Contains(x.Parent.Id))
            // .With(query, x => x.AddAscending(y => y.Id))
            .OrderBy(x => x.Id)
            .ToBatchPageAsync(x => x.Parent!.Id, paging, cancellation);
    }
}