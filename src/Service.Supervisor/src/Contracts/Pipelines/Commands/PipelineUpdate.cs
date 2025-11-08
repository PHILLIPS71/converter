using FluentValidation;

namespace Giantnodes.Service.Supervisor.Contracts.Pipelines;

public sealed class PipelineUpdate
{
    public sealed record Command
    {
        public required Guid Id { get; init; }

        public required string Name { get; init; }

        public string? Description { get; init; }

        public required string Definition { get; init; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();

            RuleFor(p => p.Name)
                .NotEmpty();

            RuleFor(p => p.Definition)
                .NotEmpty();
        }
    }

    public sealed record Result
    {
        public required Guid PipelineId { get; init; }
    }
}
