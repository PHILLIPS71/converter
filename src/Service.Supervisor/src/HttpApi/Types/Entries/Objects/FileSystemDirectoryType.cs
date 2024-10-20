using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Interfaces;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
using HotChocolate.Pagination;
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
            .Field(f => f.Id);

        descriptor
            .Field(f => f.PathInfo);

        descriptor
            .Field(f => f.CreatedAt);

        descriptor
            .Field(f => f.UpdatedAt);
    }

    [NodeResolver]
    internal static Task<FileSystemDirectory?> GetDirectoryByIdAsync(
        Guid id,
        ISelection selection,
        IDirectoryByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.Select(selection).LoadAsync(id, cancellation);
    }

    [UsePaging]
    internal static Task<Connection<FileSystemEntry>> GetEntriesAsync(
        [Parent] FileSystemDirectory directory,
        PagingArguments paging,
        ISelection selection,
        IEntriesByDirectoryIdDataLoader dataloader,
        CancellationToken cancellation = default)
    {
        return dataloader
            .WithPagingArguments(paging)
            .Select(selection)
            .LoadAsync(directory.Id, cancellation)
            .ToConnectionAsync();
    }

    [DataLoader]
    internal static Task<Dictionary<Guid, FileSystemDirectory>> GetDirectoryByIdAsync(
        IReadOnlyList<Guid> keys,
        ISelectorBuilder selector,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Directories
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .Select(x => x.Id, selector)
            .ToDictionaryAsync(x => x.Id, cancellation);
    }

    [DataLoader]
    internal static ValueTask<Dictionary<Guid, Page<FileSystemEntry>>> GetEntriesByDirectoryIdAsync(
        IReadOnlyList<Guid> keys,
        PagingArguments paging,
        ISelectorBuilder selector,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Entries
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .Select(x => x.Id, selector)
            .ToBatchPageAsync(x => x.Id, paging, cancellation);
    }
}