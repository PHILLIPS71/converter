using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Values;

public abstract record FileStream : ValueObject
{
    public int Index { get; init; }

    public string Codec { get; init; }

    protected FileStream()
    {
    }

    protected FileStream(int index, string codec)
    {
        Index = index;
        Codec = codec;
    }
}
