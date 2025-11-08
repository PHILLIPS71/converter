using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineStepDefinition
{
    /// <summary>
    /// The unique identifier of the step within a pipeline stage
    /// </summary>
    public string Id { get; init; } = NewId.NextSequentialGuid().ToString();

    /// <summary>
    /// The name of the step
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The type of operation to use
    /// </summary>
    public string Uses { get; init; } = string.Empty;

    /// <summary>
    /// The configuration properties for the step
    /// </summary>
    public Dictionary<string, object> With { get; init; } = [];
}
