using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class FileRepository : Repository<FileSystemFile, Id, ApplicationDbContext>, IFileRepository
{
    public FileRepository(ApplicationDbContext database)
        : base(database)
    {
    }

    public override IQueryable<FileSystemFile> ToQueryable()
    {
        return Database
            .Files
            .Include(x => x.VideoStreams)
            .Include(x => x.AudioStreams)
            .Include(x => x.SubtitleStreams)
            .AsQueryable();
    }
}
