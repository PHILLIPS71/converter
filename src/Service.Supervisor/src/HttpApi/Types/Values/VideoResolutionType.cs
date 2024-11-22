using Giantnodes.Service.Supervisor.Domain.Enumerations;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Values;

[ObjectType<VideoResolution>]
public static partial class VideoResolutionType
{
    static partial void Configure(IObjectTypeDescriptor<VideoResolution> descriptor)
    {
        descriptor
            .Field(f => f.Id);

        descriptor
            .Field(f => f.Name);

        descriptor
            .Field(f => f.Abbreviation);

        descriptor
            .Field(f => f.Width);

        descriptor
            .Field(f => f.Height);
    }
}