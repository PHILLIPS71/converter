namespace Giantnodes.Infrastructure;

/// <summary>
/// Executes all registered Unit of Work interceptors.
/// </summary>
internal sealed class UnitOfWorkExecutor : IUnitOfWorkExecutor
{
    private readonly IEnumerable<IUnitOfWorkInterceptor> _interceptors;

    public UnitOfWorkExecutor(IEnumerable<IUnitOfWorkInterceptor> interceptors)
    {
        _interceptors = interceptors;
    }

    /// <inheritdoc />
    public Task OnAfterBeginAsync(UnitOfWork uow, CancellationToken cancellation = default)
    {
        return Task.WhenAll(_interceptors.Select(x => x.OnAfterBeginAsync(uow, cancellation)));
    }

    /// <inheritdoc />
    public Task OnAfterCommitAsync(UnitOfWork uow, CancellationToken cancellation = default)
    {
        return Task.WhenAll(_interceptors.Select(x => x.OnAfterCommitAsync(uow, cancellation)));
    }
}
