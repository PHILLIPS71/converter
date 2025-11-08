namespace Giantnodes.Infrastructure;

public interface IUnitOfWorkExecutor
{
    /// <summary>
    /// Executes all interceptors' OnAfterBeginAsync methods concurrently.
    /// </summary>
    /// <param name="uow">The Unit of Work instance.</param>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default);

    /// <summary>
    /// Executes all interceptors' OnAfterCommitAsync methods concurrently.
    /// </summary>
    /// <param name="uow">The Unit of Work instance.</param>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default);
}
