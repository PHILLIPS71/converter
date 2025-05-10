using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure.Pipelines;

public static class Setup
{
    public static IServiceCollectionConfigurator UsingPipelines(
        this IServiceCollectionConfigurator collection,
        Action<IPipelineConfigurer>? configure = null)
    {
        collection.Services.TryAddSingleton<IYamlPipelineBuilder, YamlPipelineBuilder>();
        collection.Services.TryAddSingleton<IPipelineOperationFactory, PipelineOperationFactory>();

        var builder = new PipelineConfigurer(collection.Services);
        configure?.Invoke(builder);

        return collection;
    }
}