namespace Giantnodes.Infrastructure;

/// <summary>
/// Defines the contract for a Unit of Work interceptor.
/// </summary>
public interface IUnitOfWorkInterceptor
{
    /// <summary>
    /// Executes after the Unit of Work begins.
    /// </summary>
    /// <param name="uow">The Unit of Work instance.</param>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default);

    /// <summary>
    /// Executes after the Unit of Work commits.
    /// </summary>
    /// <param name="uow">The Unit of Work instance.</param>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default);
}
