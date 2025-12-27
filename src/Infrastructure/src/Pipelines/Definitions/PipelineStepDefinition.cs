using FluentValidation;

namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineStepDefinition
{
    /// <summary>
    /// The unique identifier of the step within a pipeline stage
    /// </summary>
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The name of the step
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The type of operation to use
    /// </summary>
    public string Uses { get; init; } = string.Empty;

    /// <summary>
    /// The configuration properties for the step
    /// </summary>
    public Dictionary<string, object> With { get; init; } = [];

    /// <summary>
    /// Validator for <see cref="PipelineStepDefinition"/>.
    /// </summary>
    public sealed class Validator : AbstractValidator<PipelineStepDefinition>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("step id is required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("step name is required");

            RuleFor(x => x.Uses)
                .NotEmpty()
                .WithMessage("step uses is required");
        }
    }
}
