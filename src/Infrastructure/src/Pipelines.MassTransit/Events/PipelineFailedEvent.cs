using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed record PipelineFailedEvent : IntegrationEvent
{
    public required PipelineDefinition Definition { get; init; }

    public required PipelineContext Context { get; init; }

    public required ExceptionInfo Exceptions { get; init; }
}