using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Engine responsible for orchestrating the execution of pipeline stages based on their dependencies.
/// </summary>
public interface IPipelineEngine
{
    /// <summary>
    /// Executes the pipeline by orchestrating stages based on their dependencies.
    /// </summary>
    /// <param name="definition">The pipeline definition containing stages to execute.</param>
    /// <param name="context">The context shared between stages during execution.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>Success or errors encountered during execution.</returns>
    Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default);
}