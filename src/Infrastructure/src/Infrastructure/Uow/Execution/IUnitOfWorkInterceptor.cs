namespace Giantnodes.Infrastructure;

/// <summary>
/// Defines the contract for a unit of work interceptor.
/// </summary>
public interface IUnitOfWorkInterceptor
{
    /// <summary>
    /// Executes after the unit of work begins.
    /// </summary>
    /// <param name="uow">The unit of work instance.</param>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default);

    /// <summary>
    /// Executes after the unit of work commits.
    /// </summary>
    /// <param name="uow">The unit of work instance.</param>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default);
}
