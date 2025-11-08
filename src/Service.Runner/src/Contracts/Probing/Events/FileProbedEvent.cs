using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Runner.Contracts.Probing;

public sealed record FileProbedEvent : IntegrationEvent
{
    public required string Path { get; init; }

    public required VideoStreamInfo[] VideoStreams { get; init; } = [];

    public required AudioStreamInfo[] AudioStreams { get; init; } = [];

    public required SubtitleStreamInfo[] SubtitleStreams { get; init; } = [];

    public abstract record StreamInfo
    {
        public required int Index { get; init; }

        public required string Codec { get; init; }

        public bool Default { get; init; }

        public bool Forced { get; init; }
    }

    public abstract record MediaStreamInfo : StreamInfo
    {
        public required TimeSpan Duration { get; init; }

        public required long Bitrate { get; init; }
    }

    public sealed record VideoStreamInfo : MediaStreamInfo
    {
        public required int Width { get; init; }

        public required int Height { get; init; }

        public required double Framerate { get; init; }

        public required string AspectRatio { get; init; }

        public required string PixelFormat { get; init; }

        public int? Rotation { get; init; }
    }

    public sealed record AudioStreamInfo : MediaStreamInfo
    {
        public string? Title { get; init; }

        public string? Language { get; init; }

        public required int SampleRate { get; init; }

        public required int Channels { get; init; }
    }

    public sealed record SubtitleStreamInfo : StreamInfo
    {
        public string? Title { get; init; }

        public required string Language { get; init; }
    }
}
