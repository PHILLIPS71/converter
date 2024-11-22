namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record SubtitleStream : FileStream
{
    private SubtitleStream()
    {
    }

    public SubtitleStream(
        int index,
        string codec,
        string? title,
        string? language,
        bool @default,
        bool forced)
        : base(index, codec)
    {
        Title = title;
        Language = language;
        Default = @default;
        Forced = forced;
    }

    public string? Title { get; init; }

    public string? Language { get; init; }

    public bool Default { get; init; }

    public bool Forced { get; init; }
}