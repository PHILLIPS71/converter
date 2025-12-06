using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints.Pipelines;

[ObjectType<Pipeline>]
internal static partial class PipelineType
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
    public static async Task<Pipeline?> GetPipelineByIdAsync(
        Id id,
        QueryContext<Pipeline> query,
        IPipelineByIdDataLoader dataloader,
        CancellationToken cancellation)
        => await dataloader
            .With(query)
            .LoadAsync(id, cancellation);

    [UsePaging]
    [UseFiltering]
    internal static async Task<Connection<PipelineExecution>> GetExecutionsAsync(
        [Parent(requires: nameof(Pipeline.Id))]
        Pipeline pipeline,
        PagingArguments paging,
        QueryContext<PipelineExecution> query,
        IExecutionsByPipelineIdDataLoader dataloader,
        CancellationToken cancellation = default)
        => await dataloader
            .With(paging, query)
            .LoadAsync(pipeline.Id, cancellation)
            .ToConnectionAsync();

    [DataLoader]
    internal static async Task<Dictionary<Id, Pipeline>> GetPipelineByIdAsync(
        IReadOnlyList<Id> keys,
        QueryContext<Pipeline> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
        => await database
            .Pipelines
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query)
            .ToDictionaryAsync(x => x.Id, cancellation);

    [DataLoader]
    internal static async Task<Dictionary<Id, Page<PipelineExecution>>> GetExecutionsByPipelineIdAsync(
        IReadOnlyList<Id> keys,
        PagingArguments paging,
        QueryContext<PipelineExecution> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
        => await database
            .PipelineExecutions
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query, x => x.AddDescending(y => y.CreatedAt))
            .ToBatchPageAsync(x => x.Id, paging, cancellation);
}
