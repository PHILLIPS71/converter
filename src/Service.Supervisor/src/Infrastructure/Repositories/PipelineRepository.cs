using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.EntityFrameworkCore;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.Infrastructure.Repositories;

internal sealed class PipelineRepository : Repository<Pipeline, Id, ApplicationDbContext>, IPipelineRepository
{
    public PipelineRepository(ApplicationDbContext database)
        : base(database)
    {
    }

    public override IQueryable<Pipeline> ToQueryable()
    {
        return Database
            .Pipelines
            .AsQueryable();
    }

    public async Task<Pipeline?> GetByPipelineExecutionIdAsync(Id id, CancellationToken cancellation = default)
    {
        return await Database
            .PipelineExecutions
            .Where(x => x.Id == id)
            .Include(x => x.Pipeline.Executions.Where(e => e.Id == id))
            .Select(x => x.Pipeline)
            .FirstOrDefaultAsync(cancellation);
    }
}
