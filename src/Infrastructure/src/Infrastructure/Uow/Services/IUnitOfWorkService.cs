namespace Giantnodes.Infrastructure;

public interface IUnitOfWorkService
{
    /// <summary>
    /// Begins a new unit of work with default options.
    /// </summary>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unit of work context.</returns>
    public Task<IUnitOfWorkContext> BeginAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Begins a new unit of work with specified options.
    /// </summary>
    /// <param name="options">The options for the unit of work.</param>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unit of work context.</returns>
    public Task<IUnitOfWorkContext> BeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default);
}
