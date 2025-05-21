using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed record PipelineStartedEvent : IntegrationEvent, CorrelatedBy<Guid>
{
    public new required Guid CorrelationId { get; init; }

    public required PipelineDefinition Pipeline { get; init; }

    public required PipelineContext Context { get; init; }
}