using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Engine responsible for executing the steps within a pipeline stage.
/// </summary>
public interface IPipelineStageEngine
{
    /// <summary>
    /// Executes all steps within a stage sequentially.
    /// </summary>
    /// <param name="stage">The stage containing steps to execute.</param>
    /// <param name="context">The context shared between steps during execution.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>Success or errors encountered during step execution.</returns>
    Task<ErrorOr<Success>> ExecuteAsync(
        PipelineContext context,
        PipelineStageDefinition stage,
        CancellationToken cancellation = default);
}