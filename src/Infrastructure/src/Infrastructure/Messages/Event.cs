namespace Giantnodes.Infrastructure;

public abstract record Event : Message
{
    public DateTime RaisedAt { get; init; } = DateTime.UtcNow;
}