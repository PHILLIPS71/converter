namespace Giantnodes.Infrastructure.Pipelines;

public sealed class PipelineSpecificationExecute
{
    public sealed record Job
    {
        public required IDictionary<string, object> State { get; init; }

        public required PipelineSpecificationDefinition Specification { get; init; }
    }
}