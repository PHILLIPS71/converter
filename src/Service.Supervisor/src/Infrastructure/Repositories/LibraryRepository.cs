using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class LibraryRepository : Repository<Library, Id, ApplicationDbContext>, ILibraryRepository
{
    public LibraryRepository(ApplicationDbContext database)
        : base(database)
    {
    }

    public override IQueryable<Library> ToQueryable()
    {
        return Database
            .Libraries
            .Include(x => x.Directory)
            .AsQueryable();
    }
}
