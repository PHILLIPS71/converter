using System.Diagnostics.CodeAnalysis;

namespace Giantnodes.Infrastructure.GraphQL;

internal sealed class IdNodeIdValueSerializer : INodeIdValueSerializer
{
    public bool IsSupported(Type type)
        => type == typeof(Id) || type == typeof(Id<>);

    public NodeIdFormatterResult Format(Span<byte> buffer, object value, out int written)
    {
        if (value is Id id)
        {
            return System.Text.Encoding.UTF8.TryGetBytes(id.ToString(), buffer, out written)
                ? NodeIdFormatterResult.Success
                : NodeIdFormatterResult.BufferTooSmall;
        }

        written = 0;
        return NodeIdFormatterResult.InvalidValue;
    }

    public bool TryParse(ReadOnlySpan<byte> buffer, [NotNullWhen(true)] out object? value)
    {
        var conversion = Id.TryParse(buffer, out var result);
        value = result;
        return conversion;
    }
}
