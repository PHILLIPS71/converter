using System.Diagnostics;
using System.IO.Abstractions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Runner.Contracts.Probing;
using Giantnodes.Service.Runner.Contracts.Probing.Jobs;
using MassTransit;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;

namespace Giantnodes.Service.Runner.Components.Probing;

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

        var timer = Stopwatch.StartNew();
        _logger.LogInformation("started probing {Path} with job id {JobId}", context.Job.Path, context.JobId);

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

        await Parallel.ForEachAsync(
            files,
            context.CancellationToken,
            async (file, cancellation) =>
            {
                var interval = Stopwatch.StartNew();
                try
                {
                    var info = await FFmpeg.GetMediaInfo(file.FullName, cancellation);

                    var @event = new FileProbedEvent
                    {
                        Path = info.Path,
                        VideoStreams = info.VideoStreams
                            .Select(video => new FileProbedEvent.VideoStreamInfo
                            {
                                Index = video.Index,
                                Codec = video.Codec,
                                Default = Convert.ToBoolean(video.Default),
                                Forced = Convert.ToBoolean(video.Forced),
                                Duration = video.Duration,
                                Bitrate = video.Bitrate,
                                Width = video.Width,
                                Height = video.Height,
                                Framerate = video.Framerate,
                                AspectRatio = video.Ratio,
                                PixelFormat = video.PixelFormat,
                                Rotation = video.Rotation
                            })
                            .ToArray(),
                        AudioStreams = info.AudioStreams
                            .Select(audio => new FileProbedEvent.AudioStreamInfo
                            {
                                Index = audio.Index,
                                Codec = audio.Codec,
                                Title = audio.Title,
                                Language = audio.Language,
                                Default = Convert.ToBoolean(audio.Default),
                                Forced = Convert.ToBoolean(audio.Forced),
                                Duration = audio.Duration,
                                Bitrate = audio.Bitrate,
                                SampleRate = audio.SampleRate,
                                Channels = audio.Channels
                            })
                            .ToArray(),
                        SubtitleStreams = info.SubtitleStreams
                            .Select(subtitle => new FileProbedEvent.SubtitleStreamInfo
                            {
                                Index = subtitle.Index,
                                Codec = subtitle.Codec,
                                Title = subtitle.Title,
                                Language = subtitle.Language,
                                Default = Convert.ToBoolean(subtitle.Default),
                                Forced = Convert.ToBoolean(subtitle.Forced)
                            })
                            .ToArray()
                    };

                    await context.Publish(@event, context.CancellationToken);

                    _logger.LogInformation("successfully probed file {Path} with job id {JobId} in {Duration:000ms}", info.Path, context.JobId, interval.ElapsedMilliseconds);
                }
                catch (OperationCanceledException ex)
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

        _logger.LogInformation("completed probing {Path} with job id {JobId} in {Duration:000ms}", context.Job.Path, context.JobId, timer.ElapsedMilliseconds);
    }
}