using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Values;

[ObjectType<VideoStream>]
public static partial class VideoStreamType
{
    static partial void Configure(IObjectTypeDescriptor<VideoStream> descriptor)
    {
        descriptor
            .Field(f => f.Index);

        descriptor
            .Field(f => f.Codec);

        descriptor
            .Field(f => f.Quality);

        descriptor
            .Field(f => f.Duration);

        descriptor
            .Field(f => f.Bitrate);

        descriptor
            .Field(f => f.Framerate);

        descriptor
            .Field(f => f.PixelFormat);

        descriptor
            .Field(f => f.Default);

        descriptor
            .Field(f => f.Forced);

        descriptor
            .Field(f => f.Rotation);
    }
}
