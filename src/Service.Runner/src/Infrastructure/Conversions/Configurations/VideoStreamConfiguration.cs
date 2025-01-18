using Xabe.FFmpeg;

namespace Giantnodes.Service.Runner.Infrastructure.Conversions;

internal sealed record VideoStreamConfiguration(VideoCodec? Codec, long? Bitrate);