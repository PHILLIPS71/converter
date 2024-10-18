namespace Giantnodes.Infrastructure;

public interface IUnitOfWorkService
{
    /// <summary>
    /// Gets the current Unit of Work, if one is in progress.
    /// </summary>
    IUnitOfWork? Current { get; }

    /// <summary>
    /// Begins a new Unit of Work with default options.
    /// </summary>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Unit of Work context.</returns>
    Task<IUnitOfWorkContext> BeginAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Begins a new Unit of Work with specified options.
    /// </summary>
    /// <param name="options">The options for the Unit of Work.</param>
    /// <param name="cancellation">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Unit of Work context.</returns>
    Task<IUnitOfWorkContext> BeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default);
}