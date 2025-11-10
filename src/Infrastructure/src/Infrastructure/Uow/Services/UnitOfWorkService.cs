using System.Transactions;

namespace Giantnodes.Infrastructure;

internal sealed class UnitOfWorkService : IUnitOfWorkService
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUnitOfWorkAccessor _accessor;

    public UnitOfWorkService(IUnitOfWorkFactory factory, IUnitOfWorkAccessor accessor)
    {
        _factory = factory;
        _accessor = accessor;
    }

    /// <inheritdoc />
    public Task<IUnitOfWorkContext> BeginAsync(CancellationToken cancellation = default)
    {
        return BeginAsync(new UnitOfWorkOptions { Scope = TransactionScopeOption.Required }, cancellation);
    }

    /// <inheritdoc />
    public async Task<IUnitOfWorkContext> BeginAsync(
        UnitOfWorkOptions options,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var uow = await _factory.CreateAsync(cancellation);
        var context = await uow.BeginAsync(options, cancellation);

        _accessor.SetCurrent(context);

        uow.Completed += (_, _) => _accessor.SetCurrent(null);
        uow.Failed += (_, _) => _accessor.SetCurrent(null);
        uow.Disposed += (_, _) => _accessor.SetCurrent(null);

        return context;
    }
}
