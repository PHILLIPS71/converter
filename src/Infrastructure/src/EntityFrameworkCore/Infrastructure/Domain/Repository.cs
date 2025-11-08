namespace Giantnodes.Infrastructure.EntityFrameworkCore;

/// <summary>
/// Base repository implementation using Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
/// <typeparam name="TDbContext">The database context type.</typeparam>
public abstract class Repository<TEntity, TKey, TDbContext> : IRepository<TEntity, TKey>
    where TEntity : class, IAggregateRoot<TKey>
    where TKey : IId
    where TDbContext : DbContext
{
    protected readonly TDbContext Database;

    protected Repository(TDbContext database)
    {
        Database = database;
    }

    public abstract IQueryable<TEntity> ToQueryable();

    public virtual Task<TEntity?> FindByIdAsync(
        TKey id,
        CancellationToken cancellation = default)
    {
        var specification = new IdSpecification<TEntity, TKey>(id);
        return ToQueryable().SingleOrDefaultAsync(specification.ToExpression(), cancellation);
    }

    public virtual Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default)
    {
        return ToQueryable().Where(specification.ToExpression()).AnyAsync(cancellation);
    }

    public virtual Task<TEntity> FirstAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default)
    {
        return ToQueryable().FirstAsync(specification.ToExpression(), cancellation);
    }

    public virtual Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default)
    {
        return ToQueryable().FirstOrDefaultAsync(specification.ToExpression(), cancellation);
    }

    public virtual Task<TEntity> SingleAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleAsync(specification.ToExpression(), cancellation);
    }

    public virtual Task<TEntity?> SingleOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default)
    {
        return ToQueryable().SingleOrDefaultAsync(specification.ToExpression(), cancellation);
    }

    public virtual Task<List<TEntity>> ToListAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default)
    {
        return ToQueryable().Where(specification.ToExpression()).ToListAsync(cancellation);
    }

    public virtual TEntity Create(TEntity entity)
    {
        return Database.Set<TEntity>().Add(entity).Entity;
    }

    public virtual TEntity Update(TEntity entity)
    {
        return Database.Set<TEntity>().Update(entity).Entity;
    }

    public virtual TEntity Delete(TEntity entity)
    {
        return Database.Set<TEntity>().Remove(entity).Entity;
    }
}
