using HotChocolate.Types;

namespace Giantnodes.Infrastructure.GraphQL.Types;

[ObjectType<FaultProperty>]
public static partial class FaultPropertyType
{
    static partial void Configure(IObjectTypeDescriptor<FaultProperty> descriptor)
    {
        descriptor
            .Field(f => f.Property);

        descriptor
            .Field(f => f.Value);

        descriptor
            .Field(f => f.Validation);
    }
}
