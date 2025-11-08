using System.Linq.Expressions;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _database;

    public FileRepository(ApplicationDbContext database)
    {
        _database = database;
    }

    public IQueryable<FileSystemFile> ToQueryable()
    {
        return _database
            .Files
            .Include(x => x.VideoStreams)
            .Include(x => x.AudioStreams)
            .Include(x => x.SubtitleStreams)
            .AsQueryable();
    }

    public Task<bool> ExistsAsync(
        Expression<Func<FileSystemFile, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().AnyAsync(predicate, cancellation);
    }

    public Task<FileSystemFile> FirstAsync(
        Expression<Func<FileSystemFile, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().FirstAsync(predicate, cancellation);
    }

    public Task<FileSystemFile?> FirstOrDefaultAsync(
        Expression<Func<FileSystemFile, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().FirstOrDefaultAsync(predicate, cancellation);
    }

    public Task<FileSystemFile> SingleAsync(
        Expression<Func<FileSystemFile, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleAsync(predicate, cancellation);
    }

    public Task<FileSystemFile?> SingleOrDefaultAsync(
        Expression<Func<FileSystemFile, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleOrDefaultAsync(predicate, cancellation);
    }

    public Task<List<FileSystemFile>> ToListAsync(
        Expression<Func<FileSystemFile, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().Where(predicate).ToListAsync(cancellation);
    }

    public FileSystemFile Create(FileSystemFile entity)
    {
        return _database.Files.Add(entity).Entity;
    }

    public FileSystemFile Update(FileSystemFile entity)
    {
        return _database.Files.Update(entity).Entity;
    }

    public FileSystemFile Delete(FileSystemFile entity)
    {
        return _database.Files.Remove(entity).Entity;
    }
}
