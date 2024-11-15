using System.Linq.Expressions;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class LibraryRepository : ILibraryRepository
{
    private readonly ApplicationDbContext _database;

    public LibraryRepository(ApplicationDbContext database)
    {
        _database = database;
    }

    public IQueryable<Library> ToQueryable()
    {
        return _database
            .Libraries
            .Include(x => x.Directory)
            .AsQueryable();
    }

    public Task<bool> ExistsAsync(
        Expression<Func<Library, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().AnyAsync(predicate, cancellation);
    }

    public Task<Library> SingleAsync(
        Expression<Func<Library, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleAsync(predicate, cancellation);
    }

    public Task<Library?> SingleOrDefaultAsync(
        Expression<Func<Library, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleOrDefaultAsync(predicate, cancellation);
    }

    public Task<List<Library>> ToListAsync(
        Expression<Func<Library, bool>> predicate,
        CancellationToken cancellation = default)
    {
        return ToQueryable().Where(predicate).ToListAsync(cancellation);
    }

    public Library Create(Library entity)
    {
        return _database.Libraries.Add(entity).Entity;
    }

    public Library Update(Library entity)
    {
        return _database.Libraries.Update(entity).Entity;
    }

    public Library Delete(Library entity)
    {
        return _database.Libraries.Remove(entity).Entity;
    }
}