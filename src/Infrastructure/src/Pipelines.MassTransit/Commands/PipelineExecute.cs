using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

internal sealed class PipelineExecute
{
    public sealed record Command : CorrelatedBy<Guid>
    {
        public required Guid CorrelationId { get; init; }

        public required PipelineDefinition Pipeline { get; init; }

        public required PipelineContext Context { get; init; }
    }
}