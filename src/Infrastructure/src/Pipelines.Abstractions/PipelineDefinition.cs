namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineDefinition
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public ICollection<PipelineSpecificationDefinition> Specifications { get; init; } = [];
}