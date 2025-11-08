using System.IO.Abstractions;
using ErrorOr;
using Giantnodes.Infrastructure;
using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Events;

namespace Giantnodes.Service.Runner.Infrastructure.Conversions;

internal sealed class ConversionService : IConversionService
{
    private readonly IFileSystem _fs;
    private readonly ILogger<ConversionService> _logger;

    public ConversionService(IFileSystem fs, ILogger<ConversionService> logger)
    {
        _fs = fs;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> ConvertAsync(
        string path,
        string? extension,
        ICollection<VideoStreamConfiguration> video,
        ICollection<AudioStreamConfiguration> audio,
        ICollection<SubtitleStreamConfiguration> subtitle,
        CancellationToken cancellation = default)
    {
        var file = _fs.FileInfo.New(path);
        if (!file.Exists)
            return Error.NotFound(description: "the file does not exist");

        try
        {
            var media = await FFmpeg.GetMediaInfo(file.FullName, cancellation);
            var conversion = FFmpeg
                .Conversions
                .New()
                .SetOutput($"{_fs.Path.GetTempPath()}/{Guid.NewGuid()}{extension ?? file.Extension}")
                .SetOverwriteOutput(true)
                .UseMultiThread(true);

            foreach (var configuration in video)
            {
                var stream = media
                    .VideoStreams
                    .FirstOrDefault(x => !configuration.Bitrate.HasValue || x.Bitrate >= configuration.Bitrate.Value);

                if (stream == null)
                {
                    var constraints = new List<string>();
                    if (configuration.Bitrate.HasValue)
                        constraints.Add($"at least {configuration.Bitrate.Value}kbps bitrate");

                    var requirements = constraints.ToOxfordSequence();
                    return Error.Validation(description: $"video stream with {requirements} not found");
                }

                if (configuration.Codec.HasValue)
                    stream.SetCodec(configuration.Codec.Value);

                if (configuration.Bitrate.HasValue)
                    stream.SetBitrate(configuration.Bitrate.Value);

                conversion.AddStream(stream);
            }

            foreach (var configuration in audio)
            {
                var streams = media
                    .AudioStreams
                    .Where(x => x.Language == "*" || configuration.Language.Equals(x.Language, StringComparison.OrdinalIgnoreCase))
                    .Where(x => !configuration.Channels.HasValue || x.Channels >= configuration.Channels.Value)
                    .Where(x => !configuration.Bitrate.HasValue || x.Bitrate >= configuration.Bitrate.Value)
                    .GroupBy(x => x.Language)
                    .Select(group => group
                        .OrderBy(stream => stream, new AudioStreamComparer(configuration))
                        .First())
                    .ToList();

                if (streams.Count == 0)
                {
                    if (configuration.Optional)
                        continue;

                    var constraints = new List<string>();
                    if (configuration.Channels.HasValue)
                        constraints.Add($"at least {configuration.Channels.Value} channels");

                    if (configuration.Bitrate.HasValue)
                        constraints.Add($"at least {configuration.Bitrate.Value}kbps bitrate");

                    var requirements = constraints.ToOxfordSequence();
                    return Error.Validation(description: $"audio stream for language '{configuration.Language}' with {requirements} not found");
                }

                foreach (var stream in streams)
                {
                    if (configuration.Channels.HasValue)
                        stream.SetChannels(configuration.Channels.Value);

                    if (configuration.Bitrate.HasValue)
                        stream.SetBitrate(configuration.Bitrate.Value);

                    conversion.AddStream(stream);
                }
            }

            foreach (var configuration in subtitle)
            {
                var stream = media
                    .SubtitleStreams
                    .FirstOrDefault(x =>
                        x.Language == "*"
                    || configuration.Language.Equals(x.Language, StringComparison.OrdinalIgnoreCase));

                if (stream == null)
                {
                    if (configuration.Optional)
                        continue;

                    return Error.Validation(description: $"subtitle stream for language '{configuration.Language}' not found");
                }

                conversion.AddStream(stream);
            }

            ConversionProgressEventArgs? progress = null;
            conversion.OnProgress += (_, args) =>
            {
                if (progress?.Percent == args.Percent)
                    return;

                progress = args;
                _logger.LogInformation("file {FilePath} transcode progressed to {Percent:P}", conversion.OutputFilePath, args.Percent / 100.0f);
            };

            await conversion.Start(cancellation);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "an unexpected error occurred transcoding file {FilePath}", file.FullName);
            return Error.Unexpected(description: $"an unexpected error occurred transcoding the file: {ex.Message}");
        }

        return new Success();
    }
}
