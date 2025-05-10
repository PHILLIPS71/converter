namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed record PipelineStageStartedEvent : IntegrationEvent
{
    public required PipelineDefinition Pipeline { get; init; }

    public required PipelineStageDefinition Stage { get; init; }

    public required PipelineContext Context { get; init; }
}