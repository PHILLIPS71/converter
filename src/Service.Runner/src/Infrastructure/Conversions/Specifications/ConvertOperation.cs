using ErrorOr;
using Giantnodes.Infrastructure.Pipelines;
using Xabe.FFmpeg;

namespace Giantnodes.Service.Runner.Infrastructure.Conversions;

internal sealed class ConvertOperation : IPipelineOperation
{
    public string Name => "giantnodes/convert";

    private readonly IConversionService _conversion;

    public ConvertOperation(IConversionService conversion)
    {
        _conversion = conversion;
    }

    public async Task<ErrorOr<IReadOnlyDictionary<string, object>>> ExecuteAsync(
        PipelineContext context,
        PipelineStepDefinition step,
        CancellationToken cancellation = default)
    {
        var path = context.Get<string>("path");
        if (path.IsError)
            return path.Errors;

        var extension = step.GetOptional<string?>("extension");
        if (path.IsError)
            return path.Errors;

        var video = step.GetOptional("video", GetVideoStreamConfiguration, []);
        if (video.IsError)
            return video.Errors;

        var audio = step.GetOptional("audio", GetAudioStreamConfiguration, []);
        if (audio.IsError)
            return audio.Errors;

        var subtitle = step.GetOptional("subtitle", GetSubtitleStreamConfiguration, []);
        if (subtitle.IsError)
            return subtitle.Errors;

        await _conversion.ConvertAsync(path.Value, extension.Value, video.Value, audio.Value, subtitle.Value, cancellation);
        return new Dictionary<string, object>();
    }

    private static ErrorOr<List<VideoStreamConfiguration>> GetVideoStreamConfiguration(object @object)
    {
        if (@object is not List<object> streams)
            return Error.Validation(description: "invalid video stream configuration format");

        var results = new List<VideoStreamConfiguration>();
        foreach (var stream in streams)
        {
            if (stream is not Dictionary<string, object> properties)
                return Error.Failure(description: "invalid video stream configuration format");

            VideoCodec? codec = null;
            if (properties.TryGetValue("codec", out var property))
            {
                if (!Enum.TryParse<VideoCodec>(property.ToString(), out var value))
                    return Error.Validation(description: "codec must be a valid video codec string");

                codec = value;
            }

            long? bitrate = null;
            if (properties.TryGetValue("bitrate", out property))
            {
                if (!long.TryParse(property.ToString(), out var value))
                    return Error.Validation(description: "bitrate must be a number");

                bitrate = value;
            }

            results.Add(new VideoStreamConfiguration(codec, bitrate));
        }

        return results;
    }

    private static ErrorOr<List<AudioStreamConfiguration>> GetAudioStreamConfiguration(object @object)
    {
        if (@object is not List<object> streams)
            return Error.Validation(description: "invalid audio stream configuration format");

        var results = new List<AudioStreamConfiguration>();
        foreach (var stream in streams)
        {
            if (stream is not Dictionary<string, object> properties)
                return Error.Failure(description: "invalid audio stream configuration format");

            if (!properties.TryGetValue("language", out var property) || property is not string language)
                return Error.Validation(description: "language is required and must be a string");

            int? channels = null;
            if (properties.TryGetValue("channels", out property))
            {
                if (!int.TryParse(property.ToString(), out var value))
                    return Error.Validation(description: "channels must be a number");

                channels = value;
            }

            long? bitrate = null;
            if (properties.TryGetValue("bitrate", out property))
            {
                if (!long.TryParse(property.ToString(), out var value))
                    return Error.Validation(description: "bitrate must be a number");

                bitrate = value;
            }

            var optional = false;
            if (properties.TryGetValue("optional", out property))
            {
                if (!bool.TryParse(property.ToString(), out var value))
                    return Error.Validation(description: "optional must be true or false");

                optional = value;
            }

            results.Add(new AudioStreamConfiguration(language, channels, bitrate, optional));
        }

        return results;
    }

    private static ErrorOr<List<SubtitleStreamConfiguration>> GetSubtitleStreamConfiguration(object @object)
    {
        if (@object is not List<object> streams)
            return Error.Validation(description: "invalid subtitle stream configuration format");

        var results = new List<SubtitleStreamConfiguration>();
        foreach (var stream in streams)
        {
            if (stream is not Dictionary<string, object> properties)
                return Error.Failure(description: "invalid subtitle stream configuration format");

            if (!properties.TryGetValue("language", out var property) || property is not string language)
                return Error.Validation(description: "language is required and must be a string");

            var optional = false;
            if (properties.TryGetValue("optional", out property))
            {
                if (!bool.TryParse(property.ToString(), out var value))
                    return Error.Validation(description: "optional must be true or false");

                optional = value;
            }

            results.Add(new SubtitleStreamConfiguration(language, optional));
        }

        return results;
    }
}