namespace Giantnodes.Infrastructure.GraphQL;

[ObjectType<FaultProperty.ValidationInfo>]
public static partial class FaultPropertyValidationInfoType
{
    static partial void Configure(IObjectTypeDescriptor<FaultProperty.ValidationInfo> descriptor)
    {
        descriptor
            .Field(f => f.Rule);

        descriptor
            .Field(f => f.Reason);
    }
}
