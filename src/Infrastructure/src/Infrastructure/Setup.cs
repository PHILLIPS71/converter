using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure;

public static partial class Setup
{
    public static IServiceCollection AddPlatform(
        this IServiceCollection collection,
        Action<IServiceCollectionConfigurator>? configure = null)
    {
        var configurator = new ServiceCollectionConfigurator(collection);
        configure?.Invoke(configurator);

        return collection;
    }
}
