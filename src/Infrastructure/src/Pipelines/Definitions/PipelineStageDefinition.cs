using FluentValidation;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Represents a stage within a pipeline containing a collection of sequential steps and dependency relationships with
/// other stages.
/// </summary>
public sealed record PipelineStageDefinition
{
    /// <summary>
    /// Gets the unique identifier for this stage within the pipeline. Used for referencing in dependency declarations.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets the human-readable name of the stage.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the collection of stage identifiers that this stage depends on. This stage will only execute after all
    /// dependency stages have completed successfully.
    /// </summary>
    public ICollection<string> Needs { get; init; } = [];

    /// <summary>
    /// Gets the collection of steps to execute sequentially within this stage.
    /// </summary>
    public ICollection<PipelineStepDefinition> Steps { get; init; } = [];

    /// <summary>
    /// Validator for <see cref="PipelineStageDefinition"/>.
    /// </summary>
    public sealed class Validator : AbstractValidator<PipelineStageDefinition>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .When(x => x.Id is not null)
                .WithMessage("stage id cannot be empty");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("stage name is required");

            RuleFor(x => x.Steps)
                .NotEmpty()
                .WithMessage("stage must contain at least one step");

            RuleFor(x => x.Steps)
                .Must(steps =>
                {
                    var ids = steps.Select(x => x.Id).ToList();
                    return ids.Count == ids.Distinct().Count();
                })
                .WithMessage("step ids must be unique within the stage");

            RuleForEach(x => x.Steps)
                .SetValidator(new PipelineStepDefinition.Validator());
        }
    }
}
