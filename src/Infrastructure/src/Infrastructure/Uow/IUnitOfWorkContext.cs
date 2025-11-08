namespace Giantnodes.Infrastructure;

/// <summary>
/// Provides the execution context and lifecycle management for a unit of work.
/// </summary>
public interface IUnitOfWorkContext : IDisposable
{
    /// <summary>
    /// Unique identifier for tracking related operations within this unit of work.
    /// </summary>
    public Guid CorrelationId { get; }

    /// <summary>
    /// The user ID associated with this unit of work, if available.
    /// </summary>
    public Id? UserId { get; }

    /// <summary>
    /// The tenant ID associated with this unit of work, if available.
    /// </summary>
    public Id? TenantId { get; }

    /// <summary>
    /// The current state the unit of work.
    /// </summary>
    public UnitOfWorkState State { get; }

    /// <summary>
    /// Commits all changes made within this unit of work transaction.
    /// </summary>
    /// <param name="cancellation">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous commit operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the unit of work has already been committed.</exception>
    public Task CommitAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Rolls back all changes made within this unit of work transaction.
    /// </summary>
    /// <param name="cancellation">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous rollback operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the unit of work hasn't started or has already been committed.</exception>
    public Task RollbackAsync(CancellationToken cancellation = default);
}
