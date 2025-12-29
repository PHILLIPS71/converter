using ErrorOr;
using FluentValidation;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Represents a complete pipeline definition containing metadata and the directed acyclic graph of stages that comprise
/// the workflow. Each pipeline defines a reusable process that can be executed against different inputs.
/// </summary>
public sealed record PipelineDefinition
{
    /// <summary>
    /// Gets the human-readable name of the pipeline.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the optional description explaining the purpose and behavior of the pipeline.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the collection of stages that comprise this pipeline, indexed by stage identifier. The stages form a
    /// directed acyclic graph through their dependency relationships.
    /// </summary>
    public IDictionary<string, PipelineStageDefinition> Stages { get; init; } = new Dictionary<string, PipelineStageDefinition>();

    /// <summary>
    /// Converts the pipeline definition into a directed acyclic graph representation that can be used for
    /// dependency-aware execution ordering.
    /// </summary>
    /// <returns>A directed acyclic graph of pipeline stages, or an error if dependency validation fails.</returns>
    /// <remarks>
    /// This method validates that all stage dependencies exist and constructs the execution graph. The resulting graph
    /// can be traversed to determine the correct execution order based on stage dependencies defined in the "needs"
    /// property of each stage.
    /// </remarks>
    public ErrorOr<DirectedAcyclicGraph<PipelineStageDefinition>> ToGraph()
    {
        var graph = new DirectedAcyclicGraph<PipelineStageDefinition>();

        // first pass: add all stages as nodes
        foreach (var stage in Stages)
            graph.AddNode(stage.Value);

        // second pass: add dependency edges
        foreach (var stage in Stages)
        {
            if (stage.Value.Needs.Count == 0)
                continue;

            foreach (var need in stage.Value.Needs)
            {
                if (!Stages.TryGetValue(need, out var dependency))
                    return Error.NotFound(description: $"stage '{stage.Key}' depends on '{need}', but the dependency was not found in the pipeline definition");

                // add edge from dependency to dependent stage (dependency → stage)
                graph.AddEdge(dependency, stage.Value);
            }
        }

        return graph;
    }

    /// <summary>
    /// Validator for <see cref="PipelineDefinition"/>.
    /// </summary>
    public sealed class Validator : AbstractValidator<PipelineDefinition>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("pipeline name is required");

            RuleFor(x => x.Description)
                .NotEmpty()
                .When(x => x.Description != null)
                .WithMessage("pipeline description cannot be empty");

            RuleFor(x => x.Stages)
                .Must(stages =>
                {
                    var ids = stages.Keys;
                    return ids.Count == ids.Distinct().Count();
                })
                .WithMessage("stage ids must be unique within the pipeline");

            RuleForEach(x => x.Stages.Values)
                .SetValidator(new PipelineStageDefinition.Validator());
        }
    }
}
