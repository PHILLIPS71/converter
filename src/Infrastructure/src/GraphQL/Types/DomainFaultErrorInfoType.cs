using HotChocolate.Types;

namespace Giantnodes.Infrastructure.GraphQL.Types;

[ObjectType<DomainFault.ErrorInfo>]
public static partial class DomainFaultErrorInfoType
{
    static partial void Configure(IObjectTypeDescriptor<DomainFault.ErrorInfo> descriptor)
    {
        descriptor
            .Field(f => f.Code);

        descriptor
            .Field(f => f.Description);
    }
}