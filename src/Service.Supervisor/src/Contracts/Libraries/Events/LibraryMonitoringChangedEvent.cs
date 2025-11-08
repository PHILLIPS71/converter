using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Contracts.Libraries;

public sealed record LibraryMonitoringChangedEvent : DomainEvent
{
    public required Guid LibraryId { get; init; }

    public required bool IsMonitoring { get; init; }
}
