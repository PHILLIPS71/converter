namespace Giantnodes.Infrastructure.Pipelines.Contracts;

public sealed record PipelineStartedEvent : IntegrationEvent
{
    public required PipelineDefinition Definition { get; init; }
}