using Giantnodes.Service.Supervisor.Domain.Enumerations;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Enumerations;

[ObjectType<VideoFileContainer>]
public static partial class VideoFileContainerType
{
    static partial void Configure(IObjectTypeDescriptor<VideoFileContainer> descriptor)
    {
        descriptor
            .Field(f => f.Id);

        descriptor
            .Field(f => f.Name);

        descriptor
            .Field(f => f.Extension);

        descriptor
            .Field(f => f.Color);
    }
}
