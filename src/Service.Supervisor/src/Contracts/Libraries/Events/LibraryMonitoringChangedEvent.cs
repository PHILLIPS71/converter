using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Contracts.Libraries;

public sealed record LibraryMonitoringChangedEvent : DomainEvent
{
    public required Id LibraryId { get; init; }

    public required bool IsMonitoring { get; init; }
}
