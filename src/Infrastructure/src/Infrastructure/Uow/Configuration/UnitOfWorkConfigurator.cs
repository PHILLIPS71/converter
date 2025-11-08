using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure;

internal sealed class UnitOfWorkConfigurator : ServiceCollectionConfigurator, IUnitOfWorkConfigurator
{
    public UnitOfWorkConfigurator(IServiceCollection collection)
        : base(collection)
    {
    }

    public IUnitOfWorkConfigurator TryAddInterceptor<TInterceptor>()
        where TInterceptor : IUnitOfWorkInterceptor
    {
        if (!Services.Any(x => x.ServiceType == typeof(IUnitOfWorkInterceptor) && x.ImplementationType == typeof(TInterceptor)))
            Services.AddScoped(typeof(IUnitOfWorkInterceptor), typeof(TInterceptor));

        return this;
    }

    public IUnitOfWorkConfigurator TryAddProvider<TUnitOfWork>()
        where TUnitOfWork : IUnitOfWork
    {
        Services.TryAddTransient(typeof(IUnitOfWork), typeof(TUnitOfWork));

        return this;
    }
}
