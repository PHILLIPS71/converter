using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure.Pipelines;

internal sealed class PipelineConfigurer : IPipelineConfigurer
{
    private readonly IServiceCollection _services;

    public PipelineConfigurer(IServiceCollection services)
    {
        _services = services;
    }

    public IPipelineConfigurer AddPipeline<TPipeline, TResult>()
        where TPipeline : class, IPipeline<TResult>
    {
        _services.TryAddSingleton<IPipeline<TResult>, TPipeline>();
        _services.TryAddSingleton<TPipeline>();

        return this;
    }
}