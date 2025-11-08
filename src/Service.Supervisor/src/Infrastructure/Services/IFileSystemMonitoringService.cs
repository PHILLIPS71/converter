using ErrorOr;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Infrastructure.Services;

public interface IFileSystemMonitoringService : IApplicationService
{
    /// <summary>
    /// Attempts to start monitoring a directory for file system changes.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to raise when file system changes occur.</typeparam>
    /// <param name="path">The directory path to monitor.</param>
    /// <param name="raise">Function to convert FileSystemEventArgs to the desired event type.</param>
    /// <returns>Success boolean or an error if monitoring cannot be established.</returns>
    public ErrorOr<bool> TryMonitor<TEvent>(string path, Func<FileSystemEventArgs, TEvent> raise)
        where TEvent : class;

    /// <summary>
    /// Attempts to stop monitoring a directory.
    /// </summary>
    /// <param name="path">The directory path to stop monitoring.</param>
    /// <returns>Success boolean or an error if the path is not being monitored.</returns>
    public ErrorOr<bool> TryUnMonitor(string path);
}
