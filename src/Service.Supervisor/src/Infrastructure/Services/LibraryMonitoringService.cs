using ErrorOr;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

namespace Giantnodes.Service.Supervisor.Infrastructure.Services;

internal sealed class LibraryMonitoringService : ILibraryMonitoringService
{
    private readonly IFileSystemMonitoringService _monitor;

    public LibraryMonitoringService(IFileSystemMonitoringService monitor)
    {
        _monitor = monitor;
    }

    public ErrorOr<bool> TryMonitor(Library library)
    {
        if (!library.IsMonitoring)
            return Error.Validation("the library cannot be monitored as monitoring is not enabled");

        return _monitor.TryMonitor(
            library.Directory.PathInfo.FullName,
            @event => new LibraryFileSystemChangedEvent
            {
                LibraryId = library.Id,
                ChangeTypes = @event.ChangeType,
                FullPath = @event.FullPath
            });
    }

    public ErrorOr<bool> TryUnMonitor(Library library)
    {
        if (!library.IsMonitoring)
            return Error.Validation("the library cannot be unmonitored as monitoring is not enabled");

        return _monitor.TryUnMonitor(library.Directory.PathInfo.FullName);
    }
}
