using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Contracts.Libraries;

public sealed record LibraryFileSystemChangedEvent : IntegrationEvent
{
    public required Guid LibraryId { get; init; }

    public required string FullPath { get; init; }

    public required WatcherChangeTypes ChangeTypes { get; init; }
}