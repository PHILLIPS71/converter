using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public interface IPipelineRepository : IRepository<Pipeline, Id>
{
    public Task<Pipeline?> GetByPipelineExecutionIdAsync(Id id, CancellationToken cancellation = default);
}
