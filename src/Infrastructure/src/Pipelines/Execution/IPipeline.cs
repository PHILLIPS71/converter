using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Defines the contract for pipeline execution engines that process directed acyclic graphs of stages containing steps.
/// </summary>
public interface IPipeline
{
    /// <summary>
    /// Executes the pipeline with a new, empty context.
    /// </summary>
    /// <param name="id">The unique identifier to track the pipeline execution.</param>
    /// <param name="definition">The pipeline definition containing stages and steps to execute.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains either success or a collection of errors
    /// encountered during execution.
    /// </returns>
    /// <remarks>
    /// This overload creates a new <see cref="PipelineContext"/> internally and delegates to the overload that accepts
    /// a context parameter.
    /// </remarks>
    public Task<ErrorOr<Success>> ExecuteAsync(
        Guid id,
        PipelineDefinition definition,
        CancellationToken cancellation = default);

    /// <summary>
    /// Executes the pipeline with the specified context containing initial state and outputs.
    /// </summary>
    /// <param name="definition">The pipeline definition containing stages and steps to execute.</param>
    /// <param name="context">
    /// The context shared between stages and steps during execution. Contains initial state and will be populated with
    /// step outputs as execution progresses.
    /// </param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains either success or a collection of errors
    /// encountered during execution.
    /// </returns>
    /// <remarks>
    /// The context object is modified during execution as step outputs are captured. Stages execute in dependency
    /// order, with parallel execution when dependencies allow.
    /// </remarks>
    public Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default);
}
