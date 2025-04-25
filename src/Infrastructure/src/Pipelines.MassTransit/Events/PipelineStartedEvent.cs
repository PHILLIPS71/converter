namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed record PipelineStartedEvent : IntegrationEvent
{
    public required PipelineDefinition Definition { get; init; }

    public required  PipelineContext Context { get; init; }
}