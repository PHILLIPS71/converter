using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Defines the contract for pipeline engines responsible for orchestrating the execution of pipeline stages based on
/// their dependency relationships.
/// </summary>
public interface IPipelineEngine
{
    /// <summary>
    /// Executes the pipeline by orchestrating stages based on their dependency graph, ensuring that stages only execute
    /// when their dependencies have completed successfully.
    /// </summary>
    /// <param name="definition">The pipeline definition containing stages to execute.</param>
    /// <param name="context">
    /// The context shared between stages during execution. Stage outputs are captured and made available to dependent
    /// stages through this context.
    /// </param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains either success when all stages complete
    /// successfully, or errors if any stage fails.
    /// </returns>
    /// <remarks>
    /// Implementations should support parallel execution of independent stages while respecting dependency constraints
    /// defined in the pipeline definition.
    /// </remarks>
    public Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default);
}
