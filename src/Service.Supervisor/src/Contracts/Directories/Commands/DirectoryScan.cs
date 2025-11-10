using FluentValidation;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Contracts.Directories;

public sealed class DirectoryScan
{
    public sealed record Command
    {
        public required Id DirectoryId { get; init; }
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
        public required Id DirectoryId { get; init; }
    }
}
