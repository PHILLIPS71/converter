using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Values;

[ObjectType<AudioStream>]
public static partial class AudioStreamType
{
    static partial void Configure(IObjectTypeDescriptor<AudioStream> descriptor)
    {
        descriptor
            .Field(f => f.Index);

        descriptor
            .Field(f => f.Codec);

        descriptor
            .Field(f => f.Title);

        descriptor
            .Field(f => f.Language);

        descriptor
            .Field(f => f.Duration);

        descriptor
            .Field(f => f.Bitrate);

        descriptor
            .Field(f => f.SampleRate);

        descriptor
            .Field(f => f.Channels);

        descriptor
            .Field(f => f.Default);

        descriptor
            .Field(f => f.Forced);
    }
}
