using Giantnodes.Service.Supervisor.Domain.Enumerations;
using Giantnodes.Service.Supervisor.Domain.Values;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class DistributionRepository : IDistributionRepository
{
    private readonly ApplicationDbContext _database;

    public DistributionRepository(ApplicationDbContext database)
    {
        _database = database;
    }

    public async Task<IReadOnlyCollection<KeyValuePair<VideoFileContainer?, int>>> GetContainerAsync(
        PathInfo path,
        CancellationToken cancellation = default)
    {
        var root = new LTree(path.FullNameNormalized);

        return await _database
            .Files
            .AsNoTracking()
            .Where(x => root.IsAncestorOf(x.PathInfo.FullNameNormalized))
            .GroupBy(x => x.PathInfo.Container)
            .OrderByDescending(x => x.Count())
            .Select(x => new KeyValuePair<VideoFileContainer?, int>(x.Key, x.Count()))
            .ToListAsync(cancellation);
    }

    public async Task<IReadOnlyCollection<KeyValuePair<string?, int>>> GetCodecAsync(
        PathInfo path,
        CancellationToken cancellation = default)
    {
        var root = new LTree(path.FullNameNormalized);

        return await _database
            .Files
            .AsNoTracking()
            .Where(x => root.IsAncestorOf(x.PathInfo.FullNameNormalized))
            .GroupBy(x => x.VideoStreams
                .Where(stream => stream.Index == 0)
                .Select(stream => stream.Codec)
                .DefaultIfEmpty()
                .FirstOrDefault())
            .OrderByDescending(x => x.Count())
            .Select(x => new KeyValuePair<string?, int>(x.Key, x.Count()))
            .ToListAsync(cancellation);
    }

    public async Task<IReadOnlyCollection<KeyValuePair<VideoResolution?, int>>> GetResolutionAsync(
        PathInfo path,
        CancellationToken cancellation = default)
    {
        var root = new LTree(path.FullNameNormalized);

        return await _database
            .Files
            .AsNoTracking()
            .Where(x => root.IsAncestorOf(x.PathInfo.FullNameNormalized))
            .GroupBy(x => x.VideoStreams
                .Where(stream => stream.Index == 0)
                .Select(stream => stream.Quality.Resolution)
                .DefaultIfEmpty()
                .FirstOrDefault())
            .OrderByDescending(x => x.Count())
            .Select(x => new KeyValuePair<VideoResolution?, int>(x.Key, x.Count()))
            .ToListAsync(cancellation);
    }
}
