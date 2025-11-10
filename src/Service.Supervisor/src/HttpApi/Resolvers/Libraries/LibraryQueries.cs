using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Libraries;

[QueryType]
internal sealed class LibraryQueries
{
    [UseSingleOrDefault]
    [UseFiltering]
    [UseSorting]
    public async Task<Library?> Library(
        ApplicationDbContext database,
        QueryContext<Library> query,
        CancellationToken cancellation = default)
        => await database
            .Libraries
            .AsNoTracking()
            .With(query, x => x.AddAscending(y => y.Id))
            .SingleOrDefaultAsync(cancellation);

    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<Connection<Library>> Libraries(
        ApplicationDbContext database,
        QueryContext<Library> query,
        PagingArguments paging,
        CancellationToken cancellation = default)
        => await database
            .Libraries
            .AsNoTracking()
            .With(query, x => x.AddAscending(y => y.Id))
            .ToPageAsync(paging, cancellation)
            .ToConnectionAsync();
}
