using FluentValidation;

namespace Giantnodes.Service.Supervisor.Contracts.Libraries;

public sealed class LibraryCreate
{
    public sealed record Command
    {
        public required string Name { get; init; }

        public required string Path { get; init; }

        public bool IsMonitoring { get; init; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.Name)
                .NotEmpty();

            RuleFor(p => p.Path)
                .NotEmpty();
        }
    }

    public sealed record Result
    {
        public required Guid LibraryId { get; init; }
    }
}