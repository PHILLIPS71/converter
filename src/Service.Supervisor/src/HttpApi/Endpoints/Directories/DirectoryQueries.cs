using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Directories;

[QueryType]
internal sealed class DirectoryQueries
{
    [UseSingleOrDefault]
    [UseFiltering]
    [UseSorting]
    public async Task<FileSystemDirectory?> Directory(
        ApplicationDbContext database,
        QueryContext<FileSystemDirectory> query,
        CancellationToken cancellation = default)
        => await database
            .Directories
            .AsNoTracking()
            .With(query, x => x.AddAscending(y => y.PathInfo.FullName))
            .SingleOrDefaultAsync(cancellation);

    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<Connection<FileSystemDirectory>> Directories(
        ApplicationDbContext database,
        QueryContext<FileSystemDirectory> query,
        PagingArguments paging,
        CancellationToken cancellation = default)
        => await database
            .Directories
            .AsNoTracking()
            .With(query, x => x.AddAscending(y => y.PathInfo.FullName))
            .ToPageAsync(paging, cancellation)
            .ToConnectionAsync();
}
