using System.Reflection;
using GreenDonut.Data.Cursors.Serializers;

namespace Giantnodes.Infrastructure.GraphQL;

internal sealed class IdCursorKeySerializer : ICursorKeySerializer
{
    private static readonly MethodInfo s_compareTo =
        typeof(Id).GetMethod(nameof(IComparable<Id>.CompareTo), [typeof(Id)]) ??
        throw new InvalidOperationException("CompareTo method not found on Id type.");

    public bool IsSupported(Type type)
        => type == typeof(Id);

    public MethodInfo GetCompareToMethod(Type type)
        => s_compareTo;

    public object Parse(ReadOnlySpan<byte> formattedKey)
    {
        if (!Id.TryParse(formattedKey, out var value))
            throw new FormatException("The cursor value is not a valid id.");

        return value;
    }

    public bool TryFormat(object key, Span<byte> buffer, out int written)
    {
        if (key is Id id)
            return System.Text.Encoding.UTF8.TryGetBytes(id.Value.ToString(), buffer, out written);

        written = 0;
        return false;
    }
}
