using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Enumerations;

public sealed record VideoFileContainer : Enumeration
{
    public string Extension { get; init; }

    private VideoFileContainer(int id, string name)
        : base(id, name)
    {
        Extension = name;
    }

    public static readonly VideoFileContainer Webm = new(1, ".webm");
    public static readonly VideoFileContainer M4V = new(2, ".m4v");
    public static readonly VideoFileContainer Nsv = new(3, ".nsv");
    public static readonly VideoFileContainer Ty = new(4, ".ty");
    public static readonly VideoFileContainer Strm = new(5, ".strm");
    public static readonly VideoFileContainer Rm = new(6, ".rm");
    public static readonly VideoFileContainer Rmbv = new(7, ".rmvb");
    public static readonly VideoFileContainer M3U = new(8, ".m3u");
    public static readonly VideoFileContainer Ifo = new(9, ".ifo");
    public static readonly VideoFileContainer Mov = new(10, ".mov");
    public static readonly VideoFileContainer Qt = new(11, ".qt");
    public static readonly VideoFileContainer Divx = new(12, ".divx");
    public static readonly VideoFileContainer Xvid = new(13, ".xvid");
    public static readonly VideoFileContainer Bivx = new(14, ".bivx");
    public static readonly VideoFileContainer Nrg = new(15, ".nrg");
    public static readonly VideoFileContainer Pva = new(16, ".pva");
    public static readonly VideoFileContainer Mwv = new(17, ".wmv");
    public static readonly VideoFileContainer Asf = new(18, ".asf");
    public static readonly VideoFileContainer Ask = new(19, ".asx");
    public static readonly VideoFileContainer Ogm = new(20, ".ogm");
    public static readonly VideoFileContainer Ogv = new(21, ".ogv");
    public static readonly VideoFileContainer M2V = new(22, ".m2v");
    public static readonly VideoFileContainer Avi = new(23, ".avi");
    public static readonly VideoFileContainer Bin = new(24, ".bin");
    public static readonly VideoFileContainer Dat = new(25, ".dat");
    public static readonly VideoFileContainer DvrMs = new(26, ".dvr-ms");
    public static readonly VideoFileContainer Mpg = new(27, ".mpg");
    public static readonly VideoFileContainer Mpeg = new(28, ".mpeg");
    public static readonly VideoFileContainer Mp4 = new(29, ".mp4");
    public static readonly VideoFileContainer Avc = new(30, ".avc");
    public static readonly VideoFileContainer Vp3 = new(31, ".vp3");
    public static readonly VideoFileContainer Svq3 = new(32, ".svq3");
    public static readonly VideoFileContainer Nuv = new(33, ".nuv");
    public static readonly VideoFileContainer Viv = new(34, ".viv");
    public static readonly VideoFileContainer Dv = new(35, ".dv");
    public static readonly VideoFileContainer Fli = new(36, ".fli");
    public static readonly VideoFileContainer Flv = new(37, ".flv");
    public static readonly VideoFileContainer Wpl = new(38, ".wpl");
    public static readonly VideoFileContainer Img = new(39, ".img");
    public static readonly VideoFileContainer Iso = new(40, ".iso");
    public static readonly VideoFileContainer Vob = new(41, ".vob");
    public static readonly VideoFileContainer Mkv = new(42, ".mkv");
    public static readonly VideoFileContainer Ts = new(43, ".ts");
    public static readonly VideoFileContainer Wtv = new(44, ".wtv");
    public static readonly VideoFileContainer M2Ts = new(45, ".m2ts");
}