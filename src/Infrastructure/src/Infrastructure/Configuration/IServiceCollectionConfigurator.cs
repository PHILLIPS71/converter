using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure;

public interface IServiceCollectionConfigurator
{
    public IServiceCollection Services { get; }
}
