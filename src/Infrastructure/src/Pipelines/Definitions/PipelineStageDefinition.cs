using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineStageDefinition
{
    public string Id { get; init; } = NewId.NextSequentialGuid().ToString();

    public string Name { get; init; } = string.Empty;

    public ICollection<string> Needs { get; init; } = [];

    public ICollection<PipelineStepDefinition> Steps { get; init; } = [];
}