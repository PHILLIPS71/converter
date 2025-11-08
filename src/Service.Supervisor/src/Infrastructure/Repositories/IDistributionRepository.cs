using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Enumerations;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

public interface IDistributionRepository : IRepository
{
    Task<IReadOnlyCollection<KeyValuePair<VideoFileContainer?, int>>> GetContainerAsync(
        PathInfo path,
        CancellationToken cancellation = default);

    Task<IReadOnlyCollection<KeyValuePair<string?, int>>> GetCodecAsync(
        PathInfo path,
        CancellationToken cancellation = default);

    Task<IReadOnlyCollection<KeyValuePair<VideoResolution?, int>>> GetResolutionAsync(
        PathInfo path,
        CancellationToken cancellation = default);
}
