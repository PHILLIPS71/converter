using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public interface IPipelineRepository : IRepository<Pipeline>
{
    Task<Pipeline?> GetByPipelineExecutionIdAsync(Guid id, CancellationToken cancellation = default);
}
