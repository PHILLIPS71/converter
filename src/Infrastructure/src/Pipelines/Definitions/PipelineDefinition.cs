using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineDefinition
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public IDictionary<string, PipelineStageDefinition> Stages { get; init; }

    public ErrorOr<DirectedAcyclicGraph<PipelineStageDefinition>> ToGraph()
    {
        var graph = new DirectedAcyclicGraph<PipelineStageDefinition>();

        foreach (var stage in Stages)
        {
            graph.AddNode(stage.Value);

            if (stage.Value.Needs.Count <= 0)
                continue;

            foreach (var need in stage.Value.Needs)
            {
                if (!Stages.TryGetValue(need, out var dependency))
                    return Error.NotFound(
                        description:
                        $"stage '{stage.Key}' depends on '{need}', but the dependency was not found in the pipeline definition");

                graph.AddNode(dependency);
                graph.AddEdge(dependency, stage.Value);
            }
        }

        return graph;
    }
}