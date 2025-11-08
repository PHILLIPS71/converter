using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Contracts.Libraries;

public sealed record LibraryCreatedEvent : DomainEvent
{
    public required Guid LibraryId { get; init; }

    public required Guid DirectoryId { get; init; }

    public required string Name { get; init; }

    public required string Slug { get; init; }
}
