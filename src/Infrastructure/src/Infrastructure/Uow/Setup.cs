using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure;

public static partial class Setup
{
    public static IServiceCollectionConfigurator UsingUow(
        this IServiceCollectionConfigurator collection,
        Action<IUnitOfWorkConfigurator> configure)
    {
        collection.Services.TryAddTransient<IUnitOfWorkService, UnitOfWorkService>();

        collection.Services.TryAddScoped<IUnitOfWorkExecutor, UnitOfWorkExecutor>();

        var configurator = new UnitOfWorkConfigurator(collection.Services);
        configure.Invoke(configurator);

        return configurator;
    }
}