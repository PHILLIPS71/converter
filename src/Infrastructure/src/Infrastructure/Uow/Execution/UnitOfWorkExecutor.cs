namespace Giantnodes.Infrastructure;

/// <summary>
/// Executes all registered unit of work interceptors during lifecycle events.
/// </summary>
internal sealed class UnitOfWorkExecutor : IUnitOfWorkExecutor
{
    private readonly IEnumerable<IUnitOfWorkInterceptor> _interceptors;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkExecutor"/> class.
    /// </summary>
    /// <param name="interceptors">The collection of interceptors to execute.</param>
    public UnitOfWorkExecutor(IEnumerable<IUnitOfWorkInterceptor> interceptors)
    {
        _interceptors = interceptors;
    }

    /// <inheritdoc />
    public Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default)
    {
        var tasks = _interceptors
            .Select(interceptor => interceptor.OnAfterBeginAsync(uow, cancellation))
            .ToArray();

        return tasks.Length == 0 ? Task.CompletedTask : Task.WhenAll(tasks);
    }

    /// <inheritdoc />
    public Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default)
    {
        var tasks = _interceptors
            .Select(interceptor => interceptor.OnAfterCommitAsync(uow, cancellation))
            .ToArray();

        return tasks.Length == 0 ? Task.CompletedTask : Task.WhenAll(tasks);
    }
}
