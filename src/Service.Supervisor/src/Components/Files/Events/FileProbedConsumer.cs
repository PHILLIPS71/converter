using Giantnodes.Infrastructure;
using Giantnodes.Service.Runner.Contracts.Probing;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Values;
using MassTransit;
using Microsoft.Extensions.Logging;
using FileStream = Giantnodes.Service.Supervisor.Domain.Values.FileStream;

namespace Giantnodes.Service.Supervisor.Components.Files.Events;

public sealed partial class FileProbedConsumer : IConsumer<Batch<FileProbedEvent>>
{
    private readonly IFileRepository _files;
    private readonly ILogger<FileProbedConsumer> _logger;

    public FileProbedConsumer(IFileRepository files, ILogger<FileProbedConsumer> logger)
    {
        _files = files;
        _logger = logger;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<Batch<FileProbedEvent>> context)
    {
        var messages = context.Message.Select(x => x.Message).ToList();

        var files = await _files
            .ToListAsync(x => messages.Select(m => m.Path).Contains(x.PathInfo.FullName), context.CancellationToken);

        foreach (var message in messages)
        {
            var file = files.SingleOrDefault(x => x.PathInfo.FullName == message.Path);
            if (file == null)
            {
                _logger.LogInformation("cannot find file {Path} to assign streams to, skipping", message.Path);
                continue;
            }

            var videos = message.VideoStreams
                .Select(x => new VideoStream(
                    index: x.Index,
                    codec: x.Codec,
                    quality: new VideoQuality(x.Width, x.Height, x.AspectRatio),
                    duration: x.Duration,
                    bitrate: x.Bitrate,
                    framerate: x.Framerate,
                    pixelFormat: x.PixelFormat,
                    @default: x.Default,
                    forced: x.Forced,
                    rotation: x.Rotation))
                .ToArray();

            var audio = message.AudioStreams
                .Select(x => new AudioStream(
                    index: x.Index,
                    codec: x.Codec,
                    title: x.Title,
                    language: x.Language,
                    duration: x.Duration,
                    bitrate: x.Bitrate,
                    sampleRate: x.SampleRate,
                    channels: x.Channels,
                    @default: x.Default,
                    forced: x.Forced))
                .ToArray();

            var subtitles = message.SubtitleStreams
                .Select(x => new SubtitleStream(
                    index: x.Index,
                    codec: x.Codec,
                    title: x.Title,
                    language: x.Language,
                    @default: x.Default,
                    forced: x.Forced))
                .ToArray();

            var streams = new List<FileStream>();
            streams.AddRange(videos);
            streams.AddRange(audio);
            streams.AddRange(subtitles);

            file.SetStreams(streams.ToArray());
        }
    }
}
