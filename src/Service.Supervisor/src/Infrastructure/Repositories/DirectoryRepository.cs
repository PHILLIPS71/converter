using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class DirectoryRepository :
    Repository<FileSystemDirectory, Id, ApplicationDbContext>,
    IDirectoryRepository
{
    public DirectoryRepository(ApplicationDbContext database)
        : base(database)
    {
    }

    public override IQueryable<FileSystemDirectory> ToQueryable()
    {
        return Database
            .Directories
            .Include(x => x.Entries)
            .AsQueryable();
    }

    public async Task<List<FileSystemFile>> GetFiles(
        FileSystemDirectory directory,
        CancellationToken cancellation = default)
    {
        var root = new LTree(directory.PathInfo.FullNameNormalized);

        return await Database
            .Files
            .Where(x => root.IsAncestorOf(x.PathInfo.FullNameNormalized))
            .ToListAsync(cancellation);
    }

    public async Task<FileSystemDirectory?> GetDirectoryHierarchy(
        Id id,
        CancellationToken cancellation = default)
    {
        var directory = await Database
            .Directories
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);

        if (directory == null)
            return null;

        // load all nested directories and their entries using LTREE hierarchy
        var root = new LTree(directory.PathInfo.FullNameNormalized);
        var directories = await Database
            .Directories
            .Include(x => x.Entries)
            .Where(x => root.IsAncestorOf(x.PathInfo.FullNameNormalized))
            .ToListAsync(cancellation);

        // return root with populated hierarchy thanks to EF Core change tracking
        return directories.Single(x => x.Id == directory.Id);
    }
}
