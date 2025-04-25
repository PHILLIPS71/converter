namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed class PipelineExecute
{
    public sealed record Command : Message
    {
        public required PipelineDefinition Definition { get; init; }

        public required IDictionary<string, object> State { get; init; }
    }
}