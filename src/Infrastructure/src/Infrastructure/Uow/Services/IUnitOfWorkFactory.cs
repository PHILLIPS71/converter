namespace Giantnodes.Infrastructure;

/// <summary>
/// Factory for creating and configuring unit of work instances.
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Creates a new unit of work.
    /// </summary>
    /// <param name="cancellation">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation and containing the started unit of work.</returns>
    public ValueTask<IUnitOfWork> CreateAsync(CancellationToken cancellation = default);
}
