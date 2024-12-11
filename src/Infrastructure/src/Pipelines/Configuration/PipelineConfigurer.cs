using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure.Pipelines;

internal sealed class PipelineConfigurer : IPipelineConfigurer
{
    private readonly PipelineOptionsBuilder _builder;

    public PipelineConfigurer(PipelineOptionsBuilder builder)
    {
        _builder = builder;
    }

    public void AddPipeline<TPipeline, TInput, TResult>()
        where TPipeline : class, IPipeline<TInput, TResult>
    {
        _builder.Services.TryAddSingleton<IPipeline<TInput, TResult>, TPipeline>();
    }
}