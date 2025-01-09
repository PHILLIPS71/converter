using System.Linq.Expressions;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class FileSystemEntryRepository : IFileSystemEntryRepository
{
    private readonly ApplicationDbContext _database;

    public FileSystemEntryRepository(ApplicationDbContext database)
    {
        _database = database;
    }

    public IQueryable<FileSystemEntry> ToQueryable()
    {
        return _database
            .Entries
            .AsQueryable();
    }

    public Task<bool> ExistsAsync(
        Expression<Func<FileSystemEntry, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().AnyAsync(predicate, cancellation);
    }

    public Task<FileSystemEntry> FirstAsync(
        Expression<Func<FileSystemEntry, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().FirstAsync(predicate, cancellation);
    }

    public Task<FileSystemEntry?> FirstOrDefaultAsync(
        Expression<Func<FileSystemEntry, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().FirstOrDefaultAsync(predicate, cancellation);
    }

    public Task<FileSystemEntry> SingleAsync(
        Expression<Func<FileSystemEntry, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleAsync(predicate, cancellation);
    }

    public Task<FileSystemEntry?> SingleOrDefaultAsync(
        Expression<Func<FileSystemEntry, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleOrDefaultAsync(predicate, cancellation);
    }

    public Task<List<FileSystemEntry>> ToListAsync(
        Expression<Func<FileSystemEntry, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().Where(predicate).ToListAsync(cancellation);
    }

    public FileSystemEntry Create(FileSystemEntry entity)
    {
        return _database.Entries.Add(entity).Entity;
    }

    public FileSystemEntry Update(FileSystemEntry entity)
    {
        return _database.Entries.Update(entity).Entity;
    }

    public FileSystemEntry Delete(FileSystemEntry entity)
    {
        return _database.Entries.Remove(entity).Entity;
    }
}