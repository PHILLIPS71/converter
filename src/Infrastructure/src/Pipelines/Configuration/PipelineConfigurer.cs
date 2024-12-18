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

    public IPipelineConfigurer AddPipeline<TInterface, TPipeline, TResult>()
        where TInterface : class, IPipeline<TResult>
        where TPipeline : class, TInterface
    {
        _services.TryAddSingleton<IPipeline<TResult>, TPipeline>();
        _services.TryAddSingleton<TInterface, TPipeline>();
        _services.TryAddSingleton<TPipeline>();

        return this;
    }

    public IPipelineConfigurer AddSpecification<TSpecification>()
        where TSpecification : IPipelineSpecification
    {
        _services.TryAddTransient(typeof(IPipelineSpecification), typeof(TSpecification));
        _services.TryAddTransient(typeof(TSpecification));

        return this;
    }
}