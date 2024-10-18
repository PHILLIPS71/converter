using HotChocolate.Types;

namespace Giantnodes.Infrastructure.GraphQL.Types;

[ObjectType<DomainFault>]
public static partial class DomainFaultType
{
    static partial void Configure(IObjectTypeDescriptor<DomainFault> descriptor)
    {
        descriptor
            .Field(f => f.RequestId);

        descriptor
            .Field(f => f.Type);

        descriptor
            .Field(f => f.Code);

        descriptor
            .Field(f => f.Message);

        descriptor
            .Field(f => f.TimeStamp);

        descriptor
            .Field(f => f.Properties);

        descriptor
            .Field(f => f.Errors);
    }
}