using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure.Pipelines;

public sealed class PipelineOptionsBuilder
{
    public IServiceCollection Services { get; }

    public PipelineOptionsBuilder(IServiceCollection services)
    {
        Services = services;
    }
}