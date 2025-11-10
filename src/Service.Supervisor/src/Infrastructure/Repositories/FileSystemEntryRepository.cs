using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class FileSystemEntryRepository : Repository<FileSystemEntry, Id, ApplicationDbContext>,
    IFileSystemEntryRepository
{
    public FileSystemEntryRepository(ApplicationDbContext database)
        : base(database)
    {
    }

    public override IQueryable<FileSystemEntry> ToQueryable()
    {
        return Database
            .Entries
            .AsQueryable();
    }
}
