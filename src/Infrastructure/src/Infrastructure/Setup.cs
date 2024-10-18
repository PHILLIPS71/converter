using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure;

public static partial class Setup
{
    public static IServiceCollection AddGiantnodes(
        this IServiceCollection collection,
        Action<IServiceCollectionConfigurator> configure)
    {
        var configurator = new ServiceCollectionConfigurator(collection);
        configure.Invoke(configurator);

        return collection;
    }
}