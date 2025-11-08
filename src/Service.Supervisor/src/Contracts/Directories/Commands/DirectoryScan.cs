using FluentValidation;

namespace Giantnodes.Service.Supervisor.Contracts.Directories;

public sealed class DirectoryScan
{
    public sealed record Command
    {
        public required Guid DirectoryId { get; init; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.DirectoryId)
                .NotEmpty();
        }
    }

    public sealed record Result
    {
        public required Guid DirectoryId { get; init; }
    }
}
