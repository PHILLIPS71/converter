using Giantnodes.Service.Supervisor.Domain.Enumerations;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Entries;

internal sealed class ResolutionDistributionEntryType : ObjectType<KeyValuePair<VideoResolution, int>>
{
    protected override void Configure(IObjectTypeDescriptor<KeyValuePair<VideoResolution, int>> descriptor)
    {
        descriptor
            .Name("ResolutionDistributionEntry");

        descriptor
            .Field(f => f.Key)
            .Type<ObjectType<VideoResolution>>();

        descriptor
            .Field(f => f.Value);
    }
}
