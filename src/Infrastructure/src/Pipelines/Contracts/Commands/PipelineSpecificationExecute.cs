namespace Giantnodes.Infrastructure.Pipelines;

public sealed class PipelineSpecificationExecute
{
    public sealed record Command
    {
        public required PipelineSpecificationDefinition Specification { get; init; }
    }
}