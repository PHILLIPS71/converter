using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure;

public class ServiceCollectionConfigurator : IServiceCollectionConfigurator
{
    public IServiceCollection Services { get; }

    public ServiceCollectionConfigurator(IServiceCollection collection)
    {
        Services = collection;
    }
}
