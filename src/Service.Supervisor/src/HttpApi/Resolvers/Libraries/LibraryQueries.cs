using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Libraries;

[QueryType]
internal sealed class LibraryQueries
{
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Library> Library(ApplicationDbContext database)
        => database.Libraries.AsNoTracking();

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Library> Libraries(ApplicationDbContext database)
        => database.Libraries.AsNoTracking();
}
