namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineStartedEvent : IntegrationEvent
{
    public required IDictionary<string, object> State { get; init; }

    public required PipelineDefinition Definition { get; init; }
}