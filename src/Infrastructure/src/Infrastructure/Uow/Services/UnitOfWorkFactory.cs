using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure;

/// <summary>
/// Factory implementation that creates unit of work instances with proper service scope management.
/// </summary>
internal sealed class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IServiceProvider _provider;

    public UnitOfWorkFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public ValueTask<IUnitOfWork> CreateAsync(CancellationToken cancellation = default)
    {
        var uow = _provider.GetRequiredService<IUnitOfWork>();
        return ValueTask.FromResult(uow);
    }
}
