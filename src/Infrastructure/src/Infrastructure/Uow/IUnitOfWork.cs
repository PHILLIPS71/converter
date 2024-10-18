namespace Giantnodes.Infrastructure;

public interface IUnitOfWork : IUnitOfWorkContext
{
    /// <summary>
    /// The options associated with this Uow.
    /// </summary>
    UnitOfWorkOptions? Options { get; }

    /// <summary>
    /// The event that is raised when this Uow has successfully completed.
    /// </summary>
    event EventHandler? Completed;

    /// <summary>
    /// The event that is raised when this Uow encounters an error.
    /// </summary>
    event EventHandler? Failed;

    /// <summary>
    /// The event that is raised when this Uow has been disposed.
    /// </summary>
    event EventHandler? Disposed;

    /// <summary>
    /// Gets if the Uow has been disposed.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Begins the Uow with the provided options.
    /// </summary>
    /// <param name="options">The options to pass this Uow.</param>
    /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    Task<IUnitOfWork> BeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default);
}