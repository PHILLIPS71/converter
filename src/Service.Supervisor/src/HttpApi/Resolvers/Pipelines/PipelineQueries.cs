using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Pipelines;

[QueryType]
internal sealed class PipelineQueries
{
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Pipeline> Pipeline(ApplicationDbContext database)
        => database.Pipelines.AsNoTracking();

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Pipeline> Pipelines(ApplicationDbContext database)
        => database.Pipelines.AsNoTracking();

    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<PipelineExecution> PipelineExecution(ApplicationDbContext database)
        => database.PipelineExecutions.AsNoTracking();

    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<PipelineExecution> PipelineExecutions(ApplicationDbContext database)
        => database.PipelineExecutions.AsNoTracking();
}