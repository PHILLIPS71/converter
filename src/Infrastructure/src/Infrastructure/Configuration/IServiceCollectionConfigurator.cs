using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure;

public interface IServiceCollectionConfigurator
{
    IServiceCollection Services { get; }
}