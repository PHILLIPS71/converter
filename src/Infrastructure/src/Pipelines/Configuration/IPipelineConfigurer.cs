using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipelineConfigurer
{
    /// <summary>
    /// Gets the service collection used to register pipeline dependencies.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Adds an operation type to the pipeline's available operations.
    /// </summary>
    /// <typeparam name="TOperation">The type of operation to add, which must implement <see cref="IPipelineOperation"/>.</typeparam>
    /// <returns>The same <see cref="IPipelineConfigurer"/> instance for method chaining.</returns>
    public IPipelineConfigurer AddOperation<TOperation>()
        where TOperation : IPipelineOperation;
}
