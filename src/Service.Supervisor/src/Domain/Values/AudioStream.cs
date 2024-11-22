namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record AudioStream : FileStream
{
    public string? Title { get; init; }

    public string? Language { get; init; }

    public TimeSpan Duration { get; init; }

    public long Bitrate { get; init; }

    public int SampleRate { get; init; }

    public int Channels { get; init; }

    public bool Default { get; init; }

    public bool Forced { get; init; }

    private AudioStream()
    {
    }
}