using HotChocolate.Language;

namespace Giantnodes.Infrastructure.GraphQL;

public sealed class IdScalar : ScalarType<Id, StringValueNode>
{
    public IdScalar()
        : this(nameof(Id))
    {
    }

    public IdScalar(string name, BindingBehavior bind = BindingBehavior.Implicit)
        : base(name, bind)
    {
    }

    public override IValueNode ParseResult(object? resultValue)
    {
        if (resultValue is null)
            return NullValueNode.Default;

        if (resultValue is string @string)
            return new StringValueNode(@string);

        if (resultValue is Id id)
            return ParseValue(id);

        throw new SerializationException(
            $"{Name} cannot parse the given literal of type `{resultValue.GetType()}`.", this);
    }

    protected override Id ParseLiteral(StringValueNode valueSyntax)
    {
        if (Id.TryParse(valueSyntax.Value, out var id))
            return id;

        throw new SerializationException(
            $"{Name} cannot parse the given literal of type `{valueSyntax.GetType()}`.", this);
    }

    protected override StringValueNode ParseValue(Id runtimeValue)
        => new(runtimeValue.ToString());
}

// public sealed class IdTypedScalar : ScalarType
// {
//     private readonly Type _type;
//
//     public IdTypedScalar(Type type)
//         : base($"{type.Name}Id", BindingBehavior.Implicit)
//     {
//         _type = type;
//     }
//
//     public override Type RuntimeType => typeof(Id<>).MakeGenericType(_type);
//
//     public override bool IsInstanceOfType(IValueNode valueSyntax)
//         => valueSyntax is StringValueNode @string && Id.TryParse(@string.Value, out _);
//
//     public override object? ParseLiteral(IValueNode valueSyntax)
//         => Activator.CreateInstance(RuntimeType, ((StringValueNode)valueSyntax).Value);
//
//     public override IValueNode ParseValue(object? runtimeValue)
//     {
//         if (runtimeValue is null)
//             return NullValueNode.Default;
//
//         return new StringValueNode(runtimeValue.ToString()!);
//     }
//
//     public override IValueNode ParseResult(object? resultValue)
//         => ParseValue(resultValue);
//
//     public override bool TrySerialize(object? runtimeValue, out object? resultValue)
//     {
//         resultValue = null;
//
//         if (runtimeValue is null)
//             return true;
//
//         if (runtimeValue.GetType() != RuntimeType)
//             return false;
//
//         resultValue = runtimeValue.ToString();
//         return true;
//     }
//
//     public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
//     {
//         runtimeValue = null;
//
//         if (resultValue is null)
//             return true;
//
//         if (resultValue is not string stringValue)
//             return false;
//
//         if (!Id.TryParse(stringValue, out _))
//             return false;
//
//         runtimeValue = Activator.CreateInstance(RuntimeType, stringValue);
//         return true;
//     }
// }
