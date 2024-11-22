using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Values;

[ObjectType<VideoQuality>]
public static partial class VideoQualityType
{
    static partial void Configure(IObjectTypeDescriptor<VideoQuality> descriptor)
    {
        descriptor
            .Field(f => f.Width);

        descriptor
            .Field(f => f.Height);

        descriptor
            .Field(f => f.AspectRatio);

        descriptor
            .Field(f => f.Resolution);
    }
}