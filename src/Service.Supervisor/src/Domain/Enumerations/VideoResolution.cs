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
        var closest = Unknown;
        var deviation = int.MaxValue;

        foreach (var quality in GetAll<VideoResolution>())
        {
            var difference = Math.Abs(quality.Width - width) + Math.Abs(quality.Height - height);
            if (difference < deviation)
            {
                deviation = difference;
                closest = quality;
            }
        }

        return closest;
    }

    // Standard Resolutions
    public static readonly VideoResolution Unknown = new(0, "Unknown", string.Empty, 0, 0);

    // Standard Definition Resolutions
    public static readonly VideoResolution Sd480 = new(1, "480p Standard Definition", "480p", 640, 480);
    public static readonly VideoResolution Sd576 = new(2, "576p Standard Definition", "576p", 720, 576);

    // High Definition Resolutions
    public static readonly VideoResolution Hd720 = new(3, "HD", "720p", 1280, 720);
    public static readonly VideoResolution Hd1080 = new(4, "Full HD", "1080p", 1920, 1080);

    // Ultra High Definition Resolutions
    public static readonly VideoResolution Uhd2K = new(5, "2K Digital Cinema", "2K", 2048, 1080);
    public static readonly VideoResolution Uhd4K = new(6, "4K Ultra High Definition", "4K UHD", 3840, 2160);
    public static readonly VideoResolution Uhd5K = new(7, "5K Ultra High Definition Plus", "5K UHD+", 5120, 2880);
    public static readonly VideoResolution Uhd8K = new(8, "8K Ultra High Definition", "8K UHD", 7680, 4320);
}