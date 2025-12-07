namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Entries;

internal sealed class CodecDistributionEntryType : ObjectType<KeyValuePair<string, int>>
{
    protected override void Configure(IObjectTypeDescriptor<KeyValuePair<string, int>> descriptor)
    {
        descriptor.Name("CodecDistributionEntry");

        descriptor
            .Field(f => f.Key)
            .Type<StringType>();

        descriptor
            .Field(f => f.Value);
    }
}
