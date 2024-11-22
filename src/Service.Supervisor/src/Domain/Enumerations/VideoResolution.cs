using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Enumerations;

public sealed record VideoResolution : Enumeration
{
    public string Abbreviation { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }

    private VideoResolution()
        : base(0, string.Empty)
    {
    }

    private VideoResolution(int id, string name, string abbreviation, int width, int height)
        : base(id, name)
    {
        Abbreviation = abbreviation;
        Width = width;
        Height = height;
    }

    public static VideoResolution FindResolution(int width, int height)
    {
        var closest = StandardDefinition;
        foreach (var quality in GetAll<VideoResolution>())
        {
            if (width >= quality.Width && height >= quality.Height)
                closest = quality;
        }

        return closest;
    }

    public static readonly VideoResolution StandardDefinition = new(1, "Standard Definition", "SD", 640, 480);
    public static readonly VideoResolution HighDefinition = new(2, "High Definition", "HD", 1280, 720);
    public static readonly VideoResolution FullHighDefinition = new(3, "Full HD", "FHD", 1920, 1080);
    public static readonly VideoResolution QuadHighDefinition = new(4, "Quad HD", "2K", 2560, 1440);
    public static readonly VideoResolution UltraHighDefinition = new(5, "Ultra HD", "4K", 3840, 2160);
    public static readonly VideoResolution FullUltraHighDefinition = new(6, "Full Ultra HD", "8K", 7680, 4320);
}