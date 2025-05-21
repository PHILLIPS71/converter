using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Base class for implementing pipelines that process a directed acyclic graph of stages containing steps.
/// </summary>
public interface IPipeline
{
    /// <summary>
    /// Executes the pipeline with a new context.
    /// </summary>
    /// <param name="definition">The pipeline definition containing stages and steps to execute.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>The result of pipeline execution or errors encountered.</returns>
    Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        CancellationToken cancellation = default);

    /// <summary>
    /// Executes the pipeline with the provided context.
    /// </summary>
    /// <param name="definition">The pipeline definition containing stages and steps to execute.</param>
    /// <param name="context">The context shared between stages and steps during execution.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>The result of pipeline execution or errors encountered.</returns>
    Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default);
}