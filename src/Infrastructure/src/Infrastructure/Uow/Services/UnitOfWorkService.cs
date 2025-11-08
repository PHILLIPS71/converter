using System.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure;

internal sealed class UnitOfWorkService : IUnitOfWorkService
{
    private readonly IServiceProvider _services;
    private readonly AsyncLocal<IUnitOfWork?> _current = new();

    public IUnitOfWork? Current => _current.Value;

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
    public async Task<IUnitOfWorkContext> BeginAsync(
        UnitOfWorkOptions options,
        CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (_current.Value != null && options.Scope == TransactionScopeOption.Required)
            return _current.Value;

        var uow = _services.GetRequiredService<IUnitOfWork>();

        uow.Completed += (sender, args) => Clean(uow);
        uow.Failed += (sender, args) => Clean(uow);
        uow.Disposed += (sender, args) => Clean(uow);

        _current.Value = await uow.BeginAsync(options, cancellation);

        return uow;
    }

    private void Clean(IUnitOfWork uow)
    {
        if (_current.Value == uow)
            _current.Value = null;
    }
}
