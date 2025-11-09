namespace Giantnodes.Infrastructure;

public interface IRepository
{
}

/// <summary>
/// A generic interface for a repository that operates on entities of type <typeparamref name="TEntity"/>, which must
/// implement the <see cref="IAggregateRoot"/> interface.
/// </summary>
/// <typeparam name="TEntity">The type of entity that this repository handles.</typeparam>
/// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
public interface IRepository<TEntity, in TKey> : IRepository
    where TEntity : IAggregateRoot<TKey>
    where TKey : IId
{
    /// <summary>
    /// Returns an <see cref="IQueryable{TEntity}"/> that can be used build and query the
    /// aggregates <typeparamref name="TEntity"/> consistency boundary.
    /// </summary>
    /// <returns>An <see cref="IQueryable{TEntity}"/> instance.</returns>
    public IQueryable<TEntity> ToQueryable();

    /// <summary>
    /// Retrieves an entity from the repository by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the entity with the specified identifier,
    /// or <c>null</c> if no entity with the identifier exists.
    /// </returns>
    public Task<TEntity?> FindByIdAsync(
        TKey id,
        CancellationToken cancellation = default);

    /// <summary>
    /// Determines whether an entity that matches the specified <paramref name="specification"/> exists in the repository.
    /// </summary>
    /// <param name="specification">A specification to test each entity.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a boolean value indicating
    /// whether an entity that matches the specified <paramref name="specification"/> exists in the repository.
    /// </returns>
    public Task<bool> AnyAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves the first entity from the repository that matches the specified <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">A specification to test each entity.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the entity that matches the specified <paramref name="specification"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when more than one entity matches the specified <paramref name="specification"/>.
    /// </exception>
    public Task<TEntity> FirstAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves the first entity from the repository that matches the specified <paramref name="specification"/>,
    /// or <c>null</c> if no entity matches the specification.
    /// </summary>
    /// <param name="specification">A specification to test each entity.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the entity that matches the
    /// specified <paramref name="specification"/>, or <c>null</c> if no entity matches the specification.
    /// </returns>
    public Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves a single entity from the repository that matches the specified <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">A specification to test each entity.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the entity that matches the
    /// specified <paramref name="specification"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when more than one entity matches the specified <paramref name="specification"/>.
    /// </exception>
    public Task<TEntity> SingleAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves a single entity from the repository that matches the specified <paramref name="specification"/>,
    /// or <c>null</c> if no entity matches the specification.
    /// </summary>
    /// <param name="specification">A specification to test each entity.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the entity that matches the
    /// specified <paramref name="specification"/>, or <c>null</c> if no entity matches the specification.
    /// </returns>
    public Task<TEntity?> SingleOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves a list of entities from the repository that match the specified <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">A specification to test each entity.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a list of entities that match
    /// the specified <paramref name="specification"/>.
    /// </returns>
    public Task<List<TEntity>> ToListAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellation = default);

    /// <summary>
    /// Creates a new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity.</returns>
    public TEntity Create(TEntity entity);

    /// <summary>
    /// Update the entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The update entity.</returns>
    public TEntity Update(TEntity entity);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>The deleted entity.</returns>
    public TEntity Delete(TEntity entity);
}
