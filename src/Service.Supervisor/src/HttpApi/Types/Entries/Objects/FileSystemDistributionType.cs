using Giantnodes.Service.Supervisor.Domain.Enumerations;
using Giantnodes.Service.Supervisor.Domain.Values;
using Giantnodes.Service.Supervisor.Infrastructure.Repositories;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Entries.Objects;

public sealed class FileSystemDistribution
{
    public PathInfo PathInfo { get; }

    public FileSystemDistribution(PathInfo pathInfo)
    {
        PathInfo = pathInfo;
    }
}

[ObjectType<FileSystemDistribution>]
public static partial class FileSystemDistributionType
{
    [GraphQLType(typeof(NonNullType<ListType<NonNullType<CodecDistributionEntryType>>>))]
    public static async Task<IReadOnlyCollection<KeyValuePair<string?, int>>?>
        GetCodecAsync(
            [Parent] FileSystemDistribution directory,
            IDistributionRepository repository,
            CancellationToken cancellation = default)
    {
        return await repository.GetCodecAsync(directory.PathInfo, cancellation);
    }

    [GraphQLType(typeof(NonNullType<ListType<NonNullType<ContainerDistributionEntryType>>>))]
    public static async Task<IReadOnlyCollection<KeyValuePair<VideoFileContainer?, int>>?>
        GetContainerAsync(
            [Parent] FileSystemDistribution directory,
            IDistributionRepository repository,
            CancellationToken cancellation = default)
    {
        return await repository.GetContainerAsync(directory.PathInfo, cancellation);
    }

    [GraphQLType(typeof(NonNullType<ListType<NonNullType<ResolutionDistributionEntryType>>>))]
    public static async Task<IReadOnlyCollection<KeyValuePair<VideoResolution?, int>>?>
        GetResolutionAsync(
            [Parent] FileSystemDistribution directory,
            IDistributionRepository repository,
            CancellationToken cancellation = default)
    {
        return await repository.GetResolutionAsync(directory.PathInfo, cancellation);
    }
}
