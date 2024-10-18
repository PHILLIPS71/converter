using System.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure;

internal sealed class UnitOfWorkService : IUnitOfWorkService
{
    private readonly IServiceProvider _services;

    /// <inheritdoc />
    public IUnitOfWork? Current { get; private set; }

    public UnitOfWorkService(IServiceProvider services)
    {
        _services = services;
    }

    /// <inheritdoc />
    public Task<IUnitOfWorkContext> BeginAsync(CancellationToken cancellation = default)
    {
        return BeginAsync(new UnitOfWorkOptions { Scope = TransactionScopeOption.Required }, cancellation);
    }

    /// <inheritdoc />
    public async Task<IUnitOfWorkContext> BeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var uow = _services.GetRequiredService<IUnitOfWork>();

        uow.Completed += (sender, args) => Current = null;

        uow.Failed += (sender, args) => Current = null;

        uow.Disposed += (sender, args) => Current = null;

        Current = await uow.BeginAsync(options, cancellation);

        return uow;
    }
}