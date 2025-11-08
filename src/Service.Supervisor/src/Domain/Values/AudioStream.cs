namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record AudioStream : FileStream
{
    private AudioStream()
    {
    }

    public AudioStream(
        int index,
        string codec,
        string? title,
        string? language,
        TimeSpan duration,
        long bitrate,
        int sampleRate,
        int channels,
        bool @default,
        bool forced)
        : base(index, codec)
    {
        Title = title;
        Language = language;
        Duration = duration;
        Bitrate = bitrate;
        SampleRate = sampleRate;
        Channels = channels;
        Default = @default;
        Forced = forced;
    }

    public string? Title { get; init; }

    public string? Language { get; init; }

    public TimeSpan Duration { get; init; }

    public long Bitrate { get; init; }

    public int SampleRate { get; init; }

    public int Channels { get; init; }

    public bool Default { get; init; }

    public bool Forced { get; init; }
}
