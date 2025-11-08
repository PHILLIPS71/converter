using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Enumerations;

public sealed record VideoFileContainer : Enumeration
{
    public string Extension { get; init; }

    public string Color { get; init; }

    private VideoFileContainer(int id, string name, string extension, string color)
        : base(id, name)
    {
        Extension = extension;
        Color = color;
    }

    public static readonly VideoFileContainer Webm = new(1, "WebM", ".webm", "#47B051");
    public static readonly VideoFileContainer M4V = new(2, "MPEG-4", ".m4v", "#0066B0");
    public static readonly VideoFileContainer Nsv = new(3, "Nullsoft Streaming", ".nsv", "#87B814");
    public static readonly VideoFileContainer Ty = new(4, "TiVo", ".ty", "#000000");
    public static readonly VideoFileContainer Strm = new(5, "Stream", ".strm", "#00B4E4");
    public static readonly VideoFileContainer Rm = new(6, "RealMedia", ".rm", "#C11E0C");
    public static readonly VideoFileContainer Rmbv = new(7, "RealMedia Variable Bitrate", ".rmvb", "#DD1812");
    public static readonly VideoFileContainer M3U = new(8, "Media Playlist", ".m3u", "#FF4081");
    public static readonly VideoFileContainer Ifo = new(9, "DVD Information", ".ifo", "#8F8F8F");
    public static readonly VideoFileContainer Mov = new(10, "QuickTime", ".mov", "#6E6E6E");
    public static readonly VideoFileContainer Qt = new(11, "QuickTime", ".qt", "#5E5E5E");
    public static readonly VideoFileContainer Divx = new(12, "DivX", ".divx", "#16499A");
    public static readonly VideoFileContainer Xvid = new(13, "XviD", ".xvid", "#087021");
    public static readonly VideoFileContainer Bivx = new(14, "BiVX", ".bivx", "#1B57B0");
    public static readonly VideoFileContainer Nrg = new(15, "Nero", ".nrg", "#01568C");
    public static readonly VideoFileContainer Pva = new(16, "MPEG PS", ".pva", "#156ABF");
    public static readonly VideoFileContainer Mwv = new(17, "Windows Media", ".wmv", "#00A4EF");
    public static readonly VideoFileContainer Asf = new(18, "Advanced Systems Format", ".asf", "#0078D7");
    public static readonly VideoFileContainer Ask = new(19, "ASF Redirector", ".asx", "#0063B1");
    public static readonly VideoFileContainer Ogm = new(20, "Ogg Media", ".ogm", "#FF7F00");
    public static readonly VideoFileContainer Ogv = new(21, "Ogg", ".ogv", "#FFA500");
    public static readonly VideoFileContainer M2V = new(22, "MPEG-2", ".m2v", "#005FB8");
    public static readonly VideoFileContainer Avi = new(23, "Audio Video Interleave", ".avi", "#00A8E1");
    public static readonly VideoFileContainer Bin = new(24, "Binary Disc", ".bin", "#666666");
    public static readonly VideoFileContainer Dat = new(25, "VCD MPEG", ".dat", "#0073E6");
    public static readonly VideoFileContainer DvrMs = new(26, "Microsoft DVR", ".dvr-ms", "#00ABF0");
    public static readonly VideoFileContainer Mpg = new(27, "MPEG", ".mpg", "#0066B0");
    public static readonly VideoFileContainer Mpeg = new(28, "MPEG", ".mpeg", "#0057A5");
    public static readonly VideoFileContainer Mp4 = new(29, "MPEG-4", ".mp4", "#0082CC");
    public static readonly VideoFileContainer Avc = new(30, "H.264/AVC", ".avc", "#005FA3");
    public static readonly VideoFileContainer Vp3 = new(31, "On2 VP3", ".vp3", "#00A94E");
    public static readonly VideoFileContainer Svq3 = new(32, "Sorenson 3", ".svq3", "#4A4A4A");
    public static readonly VideoFileContainer Nuv = new(33, "NuppelVideo", ".nuv", "#2D2D2D");
    public static readonly VideoFileContainer Viv = new(34, "VivoActive", ".viv", "#FF4081");
    public static readonly VideoFileContainer Dv = new(35, "Digital Video", ".dv", "#4A90E2");
    public static readonly VideoFileContainer Fli = new(36, "FLIC", ".fli", "#9B9B9B");
    public static readonly VideoFileContainer Flv = new(37, "Flash", ".flv", "#CC0000");
    public static readonly VideoFileContainer Wpl = new(38, "Windows Media Playlist", ".wpl", "#00B4F0");
    public static readonly VideoFileContainer Img = new(39, "Disc Image", ".img", "#465995");
    public static readonly VideoFileContainer Iso = new(40, "ISO Disc", ".iso", "#446190");
    public static readonly VideoFileContainer Vob = new(41, "DVD Video Object", ".vob", "#7A7A7A");
    public static readonly VideoFileContainer Mkv = new(42, "Matroska", ".mkv", "#4C9141");
    public static readonly VideoFileContainer Ts = new(43, "MPEG Transport Stream", ".ts", "#0091E6");
    public static readonly VideoFileContainer Wtv = new(44, "Windows Recorded TV", ".wtv", "#00A2F3");
    public static readonly VideoFileContainer M2Ts = new(45, "Blu-ray BDAV", ".m2ts", "#0046BD");
}
