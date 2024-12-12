namespace Giantnodes.Infrastructure.Pipelines.Contracts.Commands;

public sealed class PipelineSpecificationExecute
{
    public sealed record Command
    {
        public required PipelineSpecificationDefinition Specification { get; init; }
    }
}