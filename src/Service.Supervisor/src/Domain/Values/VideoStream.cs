namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record VideoStream : FileStream
{
    public VideoQuality Quality { get; init; }

    public TimeSpan Duration { get; init; }

    public long Bitrate { get; init; }

    public double Framerate { get; init; }

    public string PixelFormat { get; init; }

    public bool Default { get; init; }

    public bool Forced { get; init; }

    public int? Rotation { get; init; }
    
    private VideoStream()
    {
    }
}