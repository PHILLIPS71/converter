using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Pipelines;

[QueryType]
internal sealed class PipelineQueries
{
    [UseSingleOrDefault]
    [UseFiltering]
    [UseSorting]
    public async Task<Pipeline?> Pipeline(
        ApplicationDbContext database,
        QueryContext<Pipeline> query,
        CancellationToken cancellation = default)
        => await database
            .Pipelines
            .AsNoTracking()
            .With(query, x => x.AddAscending(y => y.Name))
            .SingleOrDefaultAsync(cancellation);

    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<Connection<Pipeline>> Pipelines(
        ApplicationDbContext database,
        QueryContext<Pipeline> query,
        PagingArguments paging,
        CancellationToken cancellation = default)
        => await database
            .Pipelines
            .AsNoTracking()
            .With(query, x => x.AddAscending(y => y.Name))
            .ToPageAsync(paging, cancellation)
            .ToConnectionAsync();

    [UseSingleOrDefault]
    [UseFiltering]
    [UseSorting]
    public async Task<PipelineExecution?> PipelineExecution(
        ApplicationDbContext database,
        QueryContext<PipelineExecution> query,
        CancellationToken cancellation = default)
        => await database
            .PipelineExecutions
            .AsNoTracking()
            .With(query, x => x.AddDescending(y => y.CreatedAt))
            .SingleOrDefaultAsync(cancellation);

    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<Connection<PipelineExecution>> PipelineExecutions(
        ApplicationDbContext database,
        QueryContext<PipelineExecution> query,
        PagingArguments paging,
        CancellationToken cancellation = default)
        => await database
            .PipelineExecutions
            .AsNoTracking()
            .With(query, x => x.AddDescending(y => y.CreatedAt))
            .ToPageAsync(paging, cancellation)
            .ToConnectionAsync();
}
