using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Defines the contract for pipeline stage engines responsible for executing the sequential steps within a pipeline stage.
/// </summary>
public interface IPipelineStageEngine
{
    /// <summary>
    /// Executes all steps within a stage sequentially, capturing outputs from each step and making them available
    /// through the pipeline context.
    /// </summary>
    /// <param name="context">
    /// The context shared between steps during execution. Step outputs are captured and stored in this context for use
    /// by subsequent stages.
    /// </param>
    /// <param name="stage">The stage definition containing steps to execute.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains either success when all steps complete
    /// successfully, or errors if any step fails.
    /// </returns>
    /// <remarks>
    /// Steps within a stage are executed sequentially in the order they are defined. If any step fails, execution stops
    /// and the error is returned immediately.
    /// </remarks>
    public Task<ErrorOr<Success>> ExecuteAsync(
        PipelineContext context,
        PipelineStageDefinition stage,
        CancellationToken cancellation = default);
}
