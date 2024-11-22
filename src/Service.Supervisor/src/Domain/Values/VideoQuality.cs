using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Enumerations;

namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record VideoQuality : ValueObject
{
    public int Width { get; init; }

    public int Height { get; init; }

    public string AspectRatio { get; init; }

    public VideoResolution Resolution { get; init; }

    private VideoQuality()
    {
    }

    public VideoQuality(int width, int height, string aspectRatio)
    {
        Width = width;
        Height = height;
        AspectRatio = aspectRatio;
        Resolution = VideoResolution.FindResolution(width, height);
    }
}