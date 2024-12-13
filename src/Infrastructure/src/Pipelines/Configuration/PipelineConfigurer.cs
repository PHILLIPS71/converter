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

    public void AddPipeline<TPipeline, TInput, TResult>()
        where TPipeline : class, IPipeline<TInput, TResult>
    {
        _services.TryAddSingleton<IPipeline<TInput, TResult>, TPipeline>();
    }
}