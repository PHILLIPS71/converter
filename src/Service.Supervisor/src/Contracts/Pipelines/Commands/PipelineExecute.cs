using FluentValidation;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Contracts.Pipelines;

public sealed class PipelineExecute
{
    public sealed record Command
    {
        public required Id PipelineId { get; init; }

        public required ICollection<Id> Entries { get; init; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.PipelineId)
                .NotEmpty();

            RuleFor(p => p.Entries)
                .NotEmpty();
        }
    }

    public sealed record Result
    {
        public record struct ExecutionResult(
            Id FileId,
            Id? PipelineExecutionId = null,
            IEnumerable<DomainFault.ErrorInfo>? Faults = null);

        public required ICollection<ExecutionResult> Executions { get; init; }
    }
}
