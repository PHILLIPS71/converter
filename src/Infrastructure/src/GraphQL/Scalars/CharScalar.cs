using HotChocolate.Language;

namespace Giantnodes.Infrastructure.GraphQL;

public sealed class CharScalar : ScalarType<char, StringValueNode>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharScalar"/> class.
    /// </summary>
    public CharScalar()
        : this("Char")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharScalar"/> class.
    /// </summary>
    public CharScalar(string name, BindingBehavior bind = BindingBehavior.Implicit)
        : base(name, bind)
    {
        Description =
            "The Char scalar type represents a display unit of information equivalent to one alphabetic letter or symbol.";
    }

    protected override bool IsInstanceOfType(StringValueNode valueSyntax)
    {
        if (char.TryParse(valueSyntax.Value, out _))
            return true;

        return false;
    }

    public override IValueNode ParseResult(object? resultValue)
    {
        if (resultValue is null)
            return NullValueNode.Default;

        if (resultValue is string s)
            return new StringValueNode(s);

        if (resultValue is char letter)
            return ParseValue(letter);

        throw new SerializationException($"{Name} cannot parse the given literal of type `{resultValue.GetType()}`.", this);
    }

    protected override char ParseLiteral(StringValueNode valueSyntax)
    {
        if (char.TryParse(valueSyntax.Value, out var letter))
            return letter;

        throw new SerializationException($"{Name} cannot parse the given literal of type `{valueSyntax.GetType()}`.", this);
    }

    protected override StringValueNode ParseValue(char runtimeValue)
    {
        return new StringValueNode(runtimeValue.ToString());
    }
}
