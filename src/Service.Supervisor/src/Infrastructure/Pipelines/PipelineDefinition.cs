namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

public sealed record PipelineDefinition
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public ICollection<PipelineSpecificationDefinition> Specifications { get; init; } = [];
}

public sealed record PipelineSpecificationDefinition
{
    public required string Name { get; init; }

    public required string Uses { get; init; }

    public required IDictionary<string, object> Properties { get; init; } = new Dictionary<string, object>();
}