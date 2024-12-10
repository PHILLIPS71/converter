namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

public sealed record PipelineDefinition
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public ICollection<IPipelineSpecification> Specifications { get; init; } = [];
}