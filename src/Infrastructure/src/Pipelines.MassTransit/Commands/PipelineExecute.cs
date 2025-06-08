using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

internal sealed class PipelineExecute
{
    public sealed record Command : Message, CorrelatedBy<Guid>
    {
        public new required Guid CorrelationId { get; init; }

        public required PipelineDefinition Pipeline { get; init; }

        public required PipelineContext Context { get; init; }
    }
}