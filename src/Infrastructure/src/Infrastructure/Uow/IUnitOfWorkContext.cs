namespace Giantnodes.Infrastructure;

public interface IUnitOfWorkContext : IAsyncDisposable
{
    /// <summary>
    /// Unique identifier for tracking related operations within this unit of work.
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// The ID of the user associated with this unit of work.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Indicates whether this unit of work has been started.
    /// </summary>
    bool IsStarted { get; }

    /// <summary>
    /// Indicates whether this unit of work has been committed.
    /// </summary>
    bool IsCommitted { get; }

    /// <summary>
    /// Commits all changes made within this unit of work.
    /// </summary>
    /// <param name="cancellation">Optional cancellation token.</param>
    Task CommitAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Rolls back all changes made within this unit of work.
    /// </summary>
    /// <param name="cancellation">Optional cancellation token.</param>
    Task RollbackAsync(CancellationToken cancellation = default);
}
