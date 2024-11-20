using FluentValidation;

namespace Giantnodes.Service.Supervisor.Contracts.Entries;

public sealed class FileSystemEntryProbe
{
    public sealed record Command
    {
        public required Guid EntryId { get; init; }
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
        public required Guid EntryId { get; init; }
    }
}