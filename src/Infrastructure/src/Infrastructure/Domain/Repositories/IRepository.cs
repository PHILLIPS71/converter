using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

/// <summary>
/// A generic interface for a repository that operates on entities of type <typeparamref name="TEntity"/>, which must
/// implement the <see cref="IAggregateRoot"/> interface.
/// </summary>
/// <typeparam name="TEntity">The type of entity that this repository handles.</typeparam>
public interface IRepository<TEntity>
    where TEntity : IAggregateRoot
{
    /// <summary>
    /// Returns an <see cref="IQueryable{TEntity}"/> that can be used build and query the
    /// aggregates <typeparamref name="TEntity"/> consistency boundary.
    /// </summary>
    /// <returns>An <see cref="IQueryable{TEntity}"/> instance.</returns>
    IQueryable<TEntity> ToQueryable();

    /// <summary>
    /// Determines whether an entity that matches the specified <paramref name="predicate"/> exists in the repository.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a boolean value indicating
    /// whether an entity that matches the specified <paramref name="predicate"/> exists in the repository.
    /// </returns>
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves a single entity from the repository that matches the specified <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the entity that matches the
    /// specified <paramref name="predicate"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when more than one entity matches the specified <paramref name="predicate"/>.
    /// </exception>
    Task<TEntity> SingleAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves a single entity from the repository that matches the specified <paramref name="predicate"/>,
    /// or <c>null</c> if no entity matches the predicate.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is the entity that matches the
    /// specified <paramref name="predicate"/>, or <c>null</c> if no entity matches the predicate.
    /// </returns>
    Task<TEntity?> SingleOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default);

    /// <summary>
    /// Retrieves a list of entities from the repository that match the specified <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">A function to test each entity for a condition.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is a list of entities that match
    /// the specified <paramref name="predicate"/>.
    /// </returns>
    Task<List<TEntity>> ToListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default);

    /// <summary>
    /// Creates a new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity.</returns>
    TEntity Create(TEntity entity);

    /// <summary>
    /// Update the entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>The update entity.</returns>
    TEntity Update(TEntity entity);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>The deleted entity.</returns>
    TEntity Delete(TEntity entity);
}