namespace Giantnodes.Service.Supervisor.Contracts.Entries;

public sealed record FileSystemChangedEvent
{
    public required string FullPath { get; init; }

    public required WatcherChangeTypes ChangeTypes { get; init; }
}