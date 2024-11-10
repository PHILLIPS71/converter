using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Directories;

[QueryType]
internal sealed class DirectoryQueries
{
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<FileSystemDirectory> Directory(ApplicationDbContext database)
        => database.Directories.AsNoTracking();

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<FileSystemDirectory> Directories(ApplicationDbContext database)
        => database.Directories.AsNoTracking();
}