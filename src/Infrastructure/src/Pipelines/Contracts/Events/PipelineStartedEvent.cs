namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineStartedEvent : IntegrationEvent
{
    public required PipelineDefinition Definition { get; init; }
}