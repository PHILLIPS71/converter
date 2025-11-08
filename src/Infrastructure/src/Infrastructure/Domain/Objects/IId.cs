namespace Giantnodes.Infrastructure;

public interface IId
{
    public Ulid Value { get; }

    public bool HasValue { get; }
}
