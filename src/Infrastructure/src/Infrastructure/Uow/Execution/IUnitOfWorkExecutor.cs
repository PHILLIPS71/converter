namespace Giantnodes.Infrastructure;

/// <summary>
/// Executes interceptors during unit of work lifecycle events.
/// </summary>
public interface IUnitOfWorkExecutor
{
    /// <summary>
    /// Executes all interceptors after a unit of work begins.
    /// </summary>
    /// <param name="uow">The unit of work instance.</param>
    /// <param name="cancellation">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default);

    /// <summary>
    /// Executes all interceptors after a unit of work commits.
    /// </summary>
    /// <param name="uow">The unit of work instance.</param>
    /// <param name="cancellation">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default);
}
