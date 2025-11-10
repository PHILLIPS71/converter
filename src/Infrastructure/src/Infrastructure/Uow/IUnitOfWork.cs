namespace Giantnodes.Infrastructure;

/// <summary>
/// Represents a unit of work that coordinates multiple operations as a single transaction.
/// </summary>
public interface IUnitOfWork : IUnitOfWorkContext
{
    /// <summary>
    /// The configuration options for this unit of work.
    /// </summary>
    public UnitOfWorkOptions? Options { get; }

    /// <summary>
    /// Occurs when the unit of work has been successfully committed.
    /// </summary>
    public event EventHandler? Completed;

    /// <summary>
    /// Occurs when the unit of work encounters an error during processing.
    /// </summary>
    public event EventHandler? Failed;

    /// <summary>
    /// Occurs when the unit of work has been disposed.
    /// </summary>
    public event EventHandler? Disposed;

    /// <summary>
    /// Begins the unit of work with the specified configuration options.
    /// </summary>
    /// <param name="options">The configuration options for this unit of work.</param>
    /// <param name="cancellation">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous begin operation. The task result contains the started unit of work.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the unit of work has already been started.</exception>
    public Task<IUnitOfWork> BeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default);
}
