using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Infrastructure.HostedServices;

internal sealed class LibraryMonitoringBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _factory;
    private readonly ILibraryMonitoringService _monitor;
    private readonly ILogger<LibraryMonitoringBackgroundService> _logger;

    public LibraryMonitoringBackgroundService(
        IServiceScopeFactory factory,
        ILibraryMonitoringService monitor,
        ILogger<LibraryMonitoringBackgroundService> logger)
    {
        _factory = factory;
        _monitor = monitor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _factory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();

            var libraries = await repository.ToListAsync(x => x.IsMonitoring, stoppingToken);
            foreach (var library in libraries)
            {
                var result = _monitor.TryMonitor(library);

                if (result.IsError)
                {
                    _logger.LogError("failed to monitor library {LibraryId} at {Path}. Error: {Error}",
                        library.Id,
                        library.Directory.PathInfo.FullName,
                        result.FirstError.Description);
                }
                else
                {
                    _logger.LogInformation("successfully started monitoring library {LibraryId} at {Path}",
                        library.Id,
                        library.Directory.PathInfo.FullName);
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "an unexpected error occurred in the library monitoring background service");
        }
    }
}
