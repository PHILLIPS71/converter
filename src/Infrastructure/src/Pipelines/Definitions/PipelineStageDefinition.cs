using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Represents a stage within a pipeline containing a collection of sequential steps and dependency relationships with
/// other stages.
/// </summary>
public sealed record PipelineStageDefinition
{
    /// <summary>
    /// Gets the unique correlation identifier for this stage instance, used for tracking during execution.
    /// </summary>
    public Guid CorrelationId { get; private set; } = NewId.NextSequentialGuid();

    /// <summary>
    /// Gets the unique identifier for this stage within the pipeline. Used for referencing in dependency declarations.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets the human-readable name of the stage.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the collection of stage identifiers that this stage depends on. This stage will only execute after all
    /// dependency stages have completed successfully.
    /// </summary>
    public ICollection<string> Needs { get; init; } = [];

    /// <summary>
    /// Gets the collection of steps to execute sequentially within this stage. All steps in a stage run on the same worker.
    /// </summary>
    public ICollection<PipelineStepDefinition> Steps { get; init; } = [];
}
