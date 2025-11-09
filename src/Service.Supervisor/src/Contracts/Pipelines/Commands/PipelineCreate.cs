using FluentValidation;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Contracts.Pipelines;

public sealed class PipelineCreate
{
    public sealed record Command
    {
        public required string Name { get; init; }

        public string? Description { get; init; }

        public required string Definition { get; init; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.Name)
                .NotEmpty();

            RuleFor(p => p.Definition)
                .NotEmpty();
        }
    }

    public sealed record Result
    {
        public required Id PipelineId { get; init; }
    }
}
