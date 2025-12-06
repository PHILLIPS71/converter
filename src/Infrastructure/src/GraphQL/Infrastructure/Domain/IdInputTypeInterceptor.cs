using System.Diagnostics.CodeAnalysis;
using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Configurations;

namespace Giantnodes.Infrastructure.GraphQL;

internal sealed class IdInputTypeInterceptor : TypeInterceptor
{
    private static readonly TypeReference s_singular = TypeReference.Create(new IdType());
    private static readonly TypeReference s_collection = TypeReference.Create(new ListType(new NonNullType(new IdType())));

    public override void OnBeforeCompleteType(
        ITypeCompletionContext context,
        TypeSystemConfiguration configuration)
    {
        if (configuration is not InputObjectTypeConfiguration input)
            return;

        foreach (var field in input.Fields)
        {
            if (field.Type is not ExtendedTypeReference type)
                continue;

            field.Type = GetIdTypeReference(type);
        }
    }

    private static TypeReference GetIdTypeReference(ExtendedTypeReference type)
    {
        var runtime = type.Type.Type;
        if (IsIdType(runtime))
            return s_singular;

        if (IsNullable(runtime, out var underlying) && IsIdType(underlying))
            return s_singular;

        if (IsCollection(runtime, out var element) && IsIdType(element))
            return s_collection;

        return type;
    }

    /// <summary>
    /// Checks if the type is <see cref="Id"/> or <see cref="Id{T}"/>.
    /// </summary>
    private static bool IsIdType(Type type)
        => type == typeof(Id) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Id<>));

    /// <summary>
    /// Checks if the type is <see cref="Nullable{T}"/> and outputs the underlying type.
    /// </summary>
    private static bool IsNullable(Type type, [NotNullWhen(true)] out Type? underlying)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
                return true;
        }

        underlying = null;
        return false;
    }

    /// <summary>
    /// Checks if the type is a collection and outputs the element type.
    /// </summary>
    private static bool IsCollection(Type type, [NotNullWhen(true)] out Type? element)
    {
        // check for array types (e.g., Id[])
        if (type.IsArray)
        {
            element = type.GetElementType();
            return element != null;
        }

        // check for generic collections that implement IEnumerable<T>
        if (type.IsGenericType)
        {
            // check if it's directly IEnumerable<T>, ICollection<T>, IList<T>, List<T>, etc.
            if (type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                element = type.GetGenericArguments()[0];
                return true;
            }

            // check the type itself if it's IEnumerable<T>
            if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                element = type.GetGenericArguments()[0];
                return true;
            }
        }

        element = null;
        return false;
    }
}
