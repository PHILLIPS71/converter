using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Shared;

[ObjectType<PathInfo>]
internal static partial class PathInfoType
{
    static partial void Configure(IObjectTypeDescriptor<PathInfo> descriptor)
    {
        descriptor
            .Field(f => f.Name);

        descriptor
            .Field(f => f.FullName);

        descriptor
            .Field(f => f.FullNameNormalized);

        descriptor
            .Field(f => f.Container);

        descriptor
            .Field(f => f.DirectoryPath);

        descriptor
            .Field(f => f.DirectorySeparatorChar);
    }
}
