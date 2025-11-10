using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Components.Libraries.Events;

public sealed partial class LibraryMonitoringChangedConsumer : IConsumer<LibraryMonitoringChangedEvent>
{
    private readonly ILibraryRepository _libraries;
    private readonly ILibraryMonitoringService _monitor;
    private readonly ILogger<LibraryMonitoringChangedConsumer> _logger;

    public LibraryMonitoringChangedConsumer(
        ILibraryRepository libraries,
        ILibraryMonitoringService monitor,
        ILogger<LibraryMonitoringChangedConsumer> logger)
    {
        _libraries = libraries;
        _monitor = monitor;
        _logger = logger;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<LibraryMonitoringChangedEvent> context)
    {
        var library = await _libraries.FindByIdAsync(context.Message.LibraryId, context.CancellationToken);
        if (library == null)
        {
            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(context.Message.LibraryId));
            return;
        }

        var result = library.IsMonitoring
            ? _monitor.TryMonitor(library)
            : _monitor.TryUnMonitor(library);

        if (result.IsError)
        {
            _logger.LogError("failed to {Operation} library {LibraryId} at {Path}. Error: {Error}",
                library.IsMonitoring ? "monitor" : "unmonitor",
                library.Id,
                library.Directory.PathInfo.FullName,
                result.FirstError.Description);
        }
        else
        {
            _logger.LogInformation("successfully {Operation} monitoring library {LibraryId} at {Path}",
                library.IsMonitoring ? "started" : "stopped",
                library.Id,
                library.Directory.PathInfo.FullName);
        }
    }
}
