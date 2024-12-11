using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure.Pipelines;

public static class Setup
{
    public static IServiceCollectionConfigurator UsingPipelines(
        this IServiceCollectionConfigurator collection,
        Action<PipelineOptionsBuilder>? configure = null)
    {
        var builder = new PipelineOptionsBuilder(collection.Services);
        configure?.Invoke(builder);

        collection.Services.TryAddSingleton<IYamlPipelineBuilder, YamlPipelineBuilder>();
        collection.Services.TryAddSingleton<IPipelineSpecificationFactory, PipelineSpecificationFactory>();

        return collection;
    }
}