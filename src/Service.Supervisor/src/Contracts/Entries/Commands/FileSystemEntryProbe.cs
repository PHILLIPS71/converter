using FluentValidation;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Contracts.Entries;

public sealed class FileSystemEntryProbe
{
    public sealed record Command
    {
        public required Id EntryId { get; init; }
    }

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(p => p.EntryId)
                .NotEmpty();
        }
    }

    public sealed record Result
    {
        public required Id EntryId { get; init; }
    }
}
