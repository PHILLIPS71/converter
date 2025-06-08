using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure.Pipelines;

public static class PipelineConfigurerExtensions
{
    /// <summary>
    /// Configures the pipeline to use in-memory execution.
    /// </summary>
    /// <param name="configurer">The <see cref="IPipelineConfigurer"/> being configured.</param>
    /// <returns>The same <see cref="IPipelineConfigurer"/> instance for method chaining.</returns>
    public static IPipelineConfigurer UseInMemory(this IPipelineConfigurer configurer)
    {
        configurer.Services.TryAddScoped<IPipeline, Pipeline>();
        configurer.Services.TryAddScoped<Pipeline>();

        return configurer;
    }
}