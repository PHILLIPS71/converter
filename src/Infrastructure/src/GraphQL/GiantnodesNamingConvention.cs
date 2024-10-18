using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace Giantnodes.Infrastructure.GraphQL;

internal sealed class GiantnodesNamingConvention : DefaultNamingConventions
{
    public override string GetTypeName(Type type, TypeKind kind)
    {
        if (type.Name != "Command")
            return base.GetTypeName(type, kind);

        // types such as UserRegister.Command need to be converted into UserRegisterInput
        var name = type.FullName?
            .Split(".")
            .LastOrDefault()?
            .Replace("+", string.Empty)
            .Replace("Command", "Input");

        if (!string.IsNullOrWhiteSpace(name))
            return name;

        return base.GetTypeName(type, kind);
    }
}