using System.Diagnostics;
using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Runner.Contracts.Probing.Jobs;
using MassTransit;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;

namespace Giantnodes.Service.Runner.Components.Probing.Jobs;

public sealed class FileSystemProbeConsumer : IJobConsumer<FileSystemProbe.Job>
{
    private readonly IFileSystem _fs;
    private readonly ILogger<FileSystemProbeConsumer> _logger;

    public FileSystemProbeConsumer(IFileSystem fs, ILogger<FileSystemProbeConsumer> logger)
    {
        _fs = fs;
        _logger = logger;
    }

    public async Task Run(JobContext<FileSystemProbe.Job> context)
    {
        var exists = _fs.Path.Exists(context.Job.Path);
        if (!exists)
        {
            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(context.Job.Path));
            return;
        }

        var files = new List<IFileInfo>();
        switch (_fs.File.GetAttributes(context.Job.Path))
        {
            case FileAttributes.Directory:
                files = _fs.DirectoryInfo
                    .New(context.Job.Path)
                    .GetFiles("*", SearchOption.AllDirectories)
                    .ToList();
                break;

            default:
                files.Add(_fs.FileInfo.New(context.Job.Path));
                break;
        }

        // todo: define a way to configure or calculate max parallel count
        await Parallel.ForEachAsync(
            files,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = 1,
                CancellationToken = context.CancellationToken
            },
            async (file, cancellation) =>
            {
                var interval = Stopwatch.StartNew();
                try
                {
                    var info = await FFmpeg.GetMediaInfo(file.FullName, cancellation);

                    _logger.LogInformation("successfully probed file {Path} with job id {JobId} in {Duration:000ms}", info.Path, context.JobId, interval.ElapsedMilliseconds);
                }
                catch (OperationCanceledException ex) when (cancellation.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "probe operation cancelled probing {Path} with job id {JobId}", file.FullName, context.JobId);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "failed to probe file {Path} with job id {JobId}", file.FullName, context.JobId);
                }
                finally
                {
                    interval.Stop();
                }
            });
    }
}