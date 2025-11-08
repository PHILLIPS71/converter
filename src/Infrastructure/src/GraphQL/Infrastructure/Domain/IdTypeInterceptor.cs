using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors;

namespace Giantnodes.Infrastructure.GraphQL;

internal sealed class IdTypeInterceptor : TypeInterceptor
{
    public override IEnumerable<TypeReference> RegisterMoreTypes(
        IReadOnlyCollection<ITypeDiscoveryContext> discoveryContexts)
    {
        return discoveryContexts
            .SelectMany(x => x.Dependencies.Select(y => y.Type))
            .OfType<ExtendedTypeReference>()
            .Where(x => x.Type.Definition is { IsGenericType: true }
                && x.Type.Definition.GetGenericTypeDefinition() == typeof(Id<>)
            )
            .Select(x => new IdTypedScalar(x.Type.TypeArguments[0].Type))
            .DistinctBy(x => x.Name)
            .Where(x => discoveryContexts.All(y => y.Type.Name != x.Name))
            .Select(x => TypeReference.Create(x));
    }
}
