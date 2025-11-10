namespace Giantnodes.Infrastructure.GraphQL;

[ObjectType<ValidationFault>]
public static partial class ValidationFaultType
{
    static partial void Configure(IObjectTypeDescriptor<ValidationFault> descriptor)
    {
        descriptor
            .Field(f => f.RequestId);

        descriptor
            .Field(f => f.Type);

        descriptor
            .Field(f => f.Code);

        descriptor
            .Field(f => f.Code);

        descriptor
            .Field(f => f.Message);

        descriptor
            .Field(f => f.TimeStamp);

        descriptor
            .Field(f => f.Properties);
    }
}
