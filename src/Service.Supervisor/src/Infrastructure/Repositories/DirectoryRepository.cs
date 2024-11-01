using System.Linq.Expressions;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class DirectoryRepository : IDirectoryRepository
{
    private readonly ApplicationDbContext _database;

    public DirectoryRepository(ApplicationDbContext database)
    {
        _database = database;
    }

    public IQueryable<FileSystemDirectory> ToQueryable()
    {
        return _database
            .Directories
            .AsQueryable();
    }

    public Task<bool> ExistsAsync(
        Expression<Func<FileSystemDirectory, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().AnyAsync(predicate, cancellation);
    }

    public Task<FileSystemDirectory> SingleAsync(
        Expression<Func<FileSystemDirectory, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleAsync(predicate, cancellation);
    }

    public Task<FileSystemDirectory?> SingleOrDefaultAsync(
        Expression<Func<FileSystemDirectory, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleOrDefaultAsync(predicate, cancellation);
    }

    public Task<List<FileSystemDirectory>> ToListAsync(
        Expression<Func<FileSystemDirectory, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().Where(predicate).ToListAsync(cancellation);
    }

    public FileSystemDirectory Create(FileSystemDirectory entity)
    {
        return _database.Directories.Add(entity).Entity;
    }

    public FileSystemDirectory Update(FileSystemDirectory entity)
    {
        return _database.Directories.Update(entity).Entity;
    }

    public FileSystemDirectory Delete(FileSystemDirectory entity)
    {
        return _database.Directories.Remove(entity).Entity;
    }
}