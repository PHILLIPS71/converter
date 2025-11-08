namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record VideoStream : FileStream
{
    private VideoStream()
    {
    }

    public VideoStream(
        int index,
        string codec,
        VideoQuality quality,
        TimeSpan duration,
        long bitrate,
        double framerate,
        string pixelFormat,
        bool @default,
        bool forced,
        int? rotation)
        : base(index, codec)
    {
        Quality = quality;
        Duration = duration;
        Bitrate = bitrate;
        Framerate = framerate;
        PixelFormat = pixelFormat;
        Default = @default;
        Forced = forced;
        Rotation = rotation;
    }

    public VideoQuality Quality { get; init; }

    public TimeSpan Duration { get; init; }

    public long Bitrate { get; init; }

    public double Framerate { get; init; }

    public string PixelFormat { get; init; }

    public bool Default { get; init; }

    public bool Forced { get; init; }

    public int? Rotation { get; init; }
}
