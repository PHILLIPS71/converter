using System.Diagnostics;
using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Components.Libraries.Events;

public sealed partial class LibraryCreateScanDirectory : IConsumer<LibraryCreatedEvent>
{
    private readonly IFileSystem _fs;
    private readonly IDirectoryRepository _directories;
    private readonly ILogger<LibraryCreateScanDirectory> _logger;

    public LibraryCreateScanDirectory(
        IFileSystem fs,
        IDirectoryRepository directories,
        ILogger<LibraryCreateScanDirectory> logger)
    {
        _fs = fs;
        _directories = directories;
        _logger = logger;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<LibraryCreatedEvent> context)
    {
        var directory = await _directories
            .SingleAsync(x => x.Id == context.Message.DirectoryId, context.CancellationToken);

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
            return;
        }

        _logger.LogInformation(
            "directory scan completed in {ElapsedTime}. DirectoryId: {DirectoryId}, Path: {DirectoryPath}", 
            Stopwatch.GetElapsedTime(stopwatch),
            directory.Id,
            directory.PathInfo.FullName);
    }
}