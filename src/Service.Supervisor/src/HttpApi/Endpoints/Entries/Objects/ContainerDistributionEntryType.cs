using Giantnodes.Service.Supervisor.Domain.Enumerations;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Entries;

internal sealed class ContainerDistributionEntryType : ObjectType<KeyValuePair<VideoFileContainer, int>>
{
    protected override void Configure(IObjectTypeDescriptor<KeyValuePair<VideoFileContainer, int>> descriptor)
    {
        descriptor.Name("ContainerDistributionEntry");

        descriptor
            .Field(f => f.Key)
            .Type<ObjectType<VideoFileContainer>>();

        descriptor
            .Field(f => f.Value);
    }
}
