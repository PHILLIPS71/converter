using System.Diagnostics;
using System.IO.Abstractions;
using ErrorOr;
using Giantnodes.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

internal sealed class DirectoryScanningService : IDirectoryScanningService
{
    private readonly IFileSystem _fs;
    private readonly IDirectoryRepository _directories;
    private readonly ILogger<DirectoryScanningService> _logger;

    public DirectoryScanningService(
        IFileSystem fs,
        IDirectoryRepository directories,
        ILogger<DirectoryScanningService> logger)
    {
        _fs = fs;
        _directories = directories;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> TryScanDirectoryAsync(Id id, CancellationToken cancellation)
    {
        var directory = await _directories.GetDirectoryHierarchy(id, cancellation);
        if (directory == null)
            return Error.NotFound(description: $"a directory with id {id} does not exist");

        var stopwatch = Stopwatch.GetTimestamp();
        var result = directory.TryScan(_fs);

        if (result.IsError)
        {
            _logger.LogError(
                "directory scan failed after {ElapsedTime}. DirectoryId: {DirectoryId}, Path: {DirectoryPath}, Error: {Error}",
                Stopwatch.GetElapsedTime(stopwatch),
                directory.Id,
                directory.PathInfo.FullName,
                result.FirstError.Description);

            return result;
        }

        _logger.LogInformation(
            "directory scan completed in {ElapsedTime}. DirectoryId: {DirectoryId}, Path: {DirectoryPath}",
            Stopwatch.GetElapsedTime(stopwatch),
            directory.Id,
            directory.PathInfo.FullName);

        return result;
    }
}
