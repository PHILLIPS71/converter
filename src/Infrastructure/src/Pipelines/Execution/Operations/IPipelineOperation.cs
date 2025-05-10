using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Interface for an operation executable.
/// </summary>
public interface IPipelineOperation
{
    string Name { get; }

    /// <summary>
    /// Executes the operation and returns a dictionary of outputs.
    /// </summary>
    /// <param name="definition">The step definition.</param>
    /// <param name="context">The pipeline context.</param>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>A result containing either a dictionary of outputs or errors.</returns>
    Task<ErrorOr<IReadOnlyDictionary<string, object>>> ExecuteAsync(
        PipelineStepDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default);
}