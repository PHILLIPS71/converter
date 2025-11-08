using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Values;

[ObjectType<SubtitleStream>]
public static partial class SubtitleStreamType
{
    static partial void Configure(IObjectTypeDescriptor<SubtitleStream> descriptor)
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
            .Field(f => f.Default);

        descriptor
            .Field(f => f.Forced);
    }
}
