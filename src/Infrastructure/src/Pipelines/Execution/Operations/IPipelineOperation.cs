using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Defines the contract for pipeline operations that can be executed as steps within pipeline stages. Operations are
/// the fundamental building blocks that perform the actual work in a pipeline.
/// </summary>
public interface IPipelineOperation
{
    /// <summary>
    /// Gets the unique name identifier for this pipeline operation. This name is used in pipeline step definitions to
    /// reference the operation.
    /// </summary>
    /// <value>
    /// A unique string identifier, typically in the format "namespace/operation-name" (e.g., "giantnodes/convert", "system/copy-file").
    /// </value>
    string Name { get; }

    /// <summary>
    /// Executes the operation with the specified step definition and pipeline context, returning any outputs produced
    /// by the operation.
    /// </summary>
    /// <param name="definition">The step definition containing configuration parameters and metadata for this execution.</param>
    /// <param name="context">The pipeline context providing access to shared state and outputs from previous steps.</param>
    /// <param name="cancellation">Token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result contains either a dictionary of named output values
    /// produced by the operation, or errors if execution fails.
    /// </returns>
    /// <remarks>
    /// Operations should be stateless and thread-safe, as they may be executed concurrently across multiple pipeline
    /// instances. All persistent state should be managed through the pipeline context or external services.
    /// </remarks>
    Task<ErrorOr<IReadOnlyDictionary<string, object>>> ExecuteAsync(
        PipelineStepDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default);
}
