using ErrorOr;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

namespace Giantnodes.Service.Supervisor.Infrastructure.Services;

public interface ILibraryMonitoringService
{
    public ErrorOr<bool> TryMonitor(Library library);

    public ErrorOr<bool> TryUnMonitor(Library library);
}
