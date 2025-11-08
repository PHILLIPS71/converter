namespace Giantnodes.Infrastructure;

public interface IUnitOfWorkConfigurator : IServiceCollectionConfigurator
{
    IUnitOfWorkConfigurator TryAddInterceptor<TInterceptor>()
        where TInterceptor : IUnitOfWorkInterceptor;

    IUnitOfWorkConfigurator TryAddProvider<TUnitOfWork>()
        where TUnitOfWork : IUnitOfWork;
}
