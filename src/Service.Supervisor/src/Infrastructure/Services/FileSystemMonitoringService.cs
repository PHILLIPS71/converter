using System.Collections.Concurrent;
using System.IO.Abstractions;
using System.Reactive.Linq;
using ErrorOr;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Infrastructure.Services;

internal sealed class FileSystemMonitoringService : IFileSystemMonitoringService, IDisposable
{
    private readonly ConcurrentDictionary<string, FileSystemMonitoringContext> _monitoring = new();
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

    private readonly IFileSystem _fs;
    private readonly IFileSystemWatcherFactory _factory;
    private readonly IBus _bus;
    private readonly ILogger<FileSystemMonitoringService> _logger;

    public FileSystemMonitoringService(
        IFileSystemWatcherFactory factory,
        IFileSystem fs,
        IBus bus,
        ILogger<FileSystemMonitoringService> logger)
    {
        _factory = factory;
        _fs = fs;
        _bus = bus;
        _logger = logger;
    }

    /// <inheritdoc />
    public ErrorOr<bool> TryMonitor<TEvent>(string path, Func<FileSystemEventArgs, TEvent> raise)
        where TEvent : class
    {
        if (_monitoring.ContainsKey(path))
            return true;

        try
        {
            if (!_fs.Directory.Exists(path))
                return Error.NotFound(description: $"a directory at '{path}' cannot be found");

            var watcher = _factory.New(path, "*");
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter =
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName |
                NotifyFilters.LastWrite |
                NotifyFilters.Size;

            // create reactive subscription to handle file system events into a single stream and samples events to
            // prevent excessively raising events
            var subscription = Observable
                .Merge(
                    Observable.FromEventPattern<FileSystemEventArgs>(watcher, nameof(watcher.Created)),
                    Observable.FromEventPattern<FileSystemEventArgs>(watcher, nameof(watcher.Renamed)),
                    Observable.FromEventPattern<FileSystemEventArgs>(watcher, nameof(watcher.Deleted))
                )
                // group events by the full path to sample each file path independently
                .GroupBy(@event => @event.EventArgs.FullPath)
                .SelectMany(group => group
                    .Sample(_interval)
                    .Select(@event => @event)
                )
                .Subscribe(
                    @event => _bus.Publish(raise(@event.EventArgs)),
                    ex => _logger.LogError(ex, "failed raising event on path {Path}, Error {Error}", path, ex.Message));

            var context = new FileSystemMonitoringContext(watcher, subscription);
            _monitoring.TryAdd(path, context);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "failed to monitor path {Path}, Error: {Error}", path, ex.Message);
            return Error.Unexpected(description: $"an unexpected error occured attempting to monitor '{path}'");
        }
    }

    /// <inheritdoc />
    public ErrorOr<bool> TryUnMonitor(string path)
    {
        if (!_monitoring.TryRemove(path, out var context))
            return Error.Validation($"the path '{path}' is not being monitored");

        context.Dispose();
        return true;
    }

    public void Dispose()
    {
        foreach (var context in _monitoring.Values)
        {
            context.Dispose();
        }

        _monitoring.Clear();
    }

    private sealed class FileSystemMonitoringContext : IDisposable
    {
        private readonly IFileSystemWatcher _watcher;
        private readonly IDisposable _subscription;

        public FileSystemMonitoringContext(IFileSystemWatcher watcher, IDisposable subscription)
        {
            _watcher = watcher;
            _subscription = subscription;
        }

        public void Dispose()
        {
            _subscription.Dispose();
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }
    }
}