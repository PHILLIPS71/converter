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
    /// <param name="step">The step definition.</param>
    /// <param name="context">The pipeline context.</param>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>A result containing either a dictionary of outputs or errors.</returns>
    Task<ErrorOr<IReadOnlyDictionary<string, object>>> ExecuteAsync(
        PipelineContext context,
        PipelineStepDefinition step,
        CancellationToken cancellation = default);
}