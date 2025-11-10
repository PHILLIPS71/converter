using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Objects;

[ObjectType<Pipeline>]
public static partial class PipelineType
{
    static partial void Configure(IObjectTypeDescriptor<Pipeline> descriptor)
    {
        descriptor
            .Field(f => f.Id);

        descriptor
            .Field(f => f.Name);

        descriptor
            .Field(f => f.Slug);

        descriptor
            .Field(f => f.Description);

        descriptor
            .Field(f => f.Definition);

        descriptor
            .Field(f => f.CreatedAt);

        descriptor
            .Field(f => f.UpdatedAt);
    }

    [NodeResolver]
    public static Task<Pipeline?> GetPipelineByIdAsync(
        Id id,
        QueryContext<Pipeline> query,
        IPipelineByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.With(query).LoadAsync(id, cancellation);
    }

    [UsePaging]
    [UseFiltering]
    internal static async Task<Connection<PipelineExecution>> GetExecutionsAsync(
        [Parent] Pipeline pipeline,
        PagingArguments paging,
        QueryContext<PipelineExecution> query,
        IExecutionsByPipelineIdDataLoader dataloader,
        CancellationToken cancellation = default)
    {
        return await dataloader
            .With(paging, query)
            .LoadAsync(pipeline.Id, cancellation)
            .ToConnectionAsync();
    }

    [DataLoader]
    internal static Task<Dictionary<Id, Pipeline>> GetPipelineByIdAsync(
        IReadOnlyList<Id> keys,
        QueryContext<Pipeline> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Pipelines
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query)
            .ToDictionaryAsync(x => x.Id, cancellation);
    }

    [DataLoader]
    internal static ValueTask<Dictionary<Id, Page<PipelineExecution>>> GetExecutionsByPipelineIdAsync(
        IReadOnlyList<Id> keys,
        PagingArguments paging,
        QueryContext<PipelineExecution> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .PipelineExecutions
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query, x => x.AddDescending(y => y.CreatedAt))
            .ToBatchPageAsync(x => x.Id, paging, cancellation);
    }
}
