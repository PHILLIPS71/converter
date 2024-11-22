namespace Giantnodes.Service.Supervisor.Domain.Values;

public sealed record SubtitleStream : FileStream
{
    public string? Title { get; init; }

    public string? Language { get; init; }

    public bool Default { get; init; }

    public bool Forced { get; init; }

    private SubtitleStream()
    {
    }
}