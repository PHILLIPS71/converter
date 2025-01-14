using FluentValidation;

namespace Giantnodes.Service.Supervisor.Contracts.Pipelines;

public sealed class PipelineExecute
{
    public sealed record Command
    {
        public required Guid PipelineId { get; init; }

        public required Guid FileId { get; init; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.PipelineId)
                .NotEmpty();

            RuleFor(p => p.FileId)
                .NotEmpty();
        }
    }

    public sealed record Result
    {
        public required Guid PipelineExecutionId { get; init; }
    }
}