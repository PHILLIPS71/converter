namespace Giantnodes.Infrastructure;

/// <summary>
/// Provides configuration methods for setting up Unit of Work services.
/// </summary>
public interface IUnitOfWorkConfigurator : IServiceCollectionConfigurator
{
    /// <summary>
    /// Registers a Unit of Work interceptor that will be executed during UoW lifecycle events. Multiple interceptors
    /// can be registered and will all be executed.
    /// </summary>
    /// <typeparam name="TInterceptor">The interceptor implementation type.</typeparam>
    /// <returns>The configurator instance for method chaining.</returns>
    /// <remarks>
    /// Uses enumerable service registration to allow multiple interceptors.
    /// Duplicate registrations of the same interceptor type are ignored.
    /// </remarks>
    public IUnitOfWorkConfigurator TryAddInterceptor<TInterceptor>()
        where TInterceptor : class, IUnitOfWorkInterceptor;

    /// <summary>
    /// Registers a Unit of Work provider implementation.
    /// </summary>
    /// <typeparam name="TUnitOfWork">The Unit of Work implementation type.</typeparam>
    /// <returns>The configurator instance for method chaining.</returns>
    /// <remarks>
    /// Only one UoW provider can be registered. Subsequent calls are ignored. The provider is registered with scoped
    /// lifetime.
    /// </remarks>
    public IUnitOfWorkConfigurator TryAddProvider<TUnitOfWork>()
        where TUnitOfWork : IUnitOfWork;
}
