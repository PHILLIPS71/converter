using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure;

public static partial class Setup
{
    /// <summary>
    /// Configures Unit of Work services and provides a fluent API for additional configuration.
    /// </summary>
    /// <param name="collection">The service collection configurator.</param>
    /// <param name="configure">Configuration action for registering UoW providers and interceptors.</param>
    /// <returns>The configurator instance for method chaining.</returns>
    /// <example>
    /// <code>
    /// services.UsingUow(configure =>
    /// {
    ///     configure
    ///         .TryAddProvider&lt;EntityFrameworkUnitOfWork&lt;MyDbContext&gt;&gt;()
    ///         .TryAddInterceptor&lt;PublishUnitOfWorkInterceptor&gt;();
    /// });
    /// </code>
    /// </example>
    public static IServiceCollectionConfigurator UsingUow(
        this IServiceCollectionConfigurator collection,
        Action<IUnitOfWorkConfigurator> configure)
    {
        collection.Services.TryAddSingleton<IUnitOfWorkAccessor, UnitOfWorkAccessor>();

        collection.Services.TryAddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        collection.Services.TryAddScoped<IUnitOfWorkService, UnitOfWorkService>();
        collection.Services.TryAddScoped<IUnitOfWorkExecutor, UnitOfWorkExecutor>();

        var configurator = new UnitOfWorkConfigurator(collection.Services);
        configure.Invoke(configurator);

        return configurator;
    }
}
