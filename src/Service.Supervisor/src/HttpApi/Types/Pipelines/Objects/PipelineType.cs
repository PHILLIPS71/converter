using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Predicates;
using GreenDonut.Selectors;
using HotChocolate.Data.Filters;
using HotChocolate.Execution.Processing;
using HotChocolate.Pagination;
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
    internal static Task<Pipeline?> GetPipelineByIdAsync(
        Guid id,
        ISelection selection,
        IPipelineByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.Select(selection).LoadAsync(id, cancellation);
    }

    [UsePaging]
    [UseFiltering]
    internal static async Task<Connection<PipelineExecution>> GetExecutionsAsync(
        [Parent] Pipeline pipeline,
        PagingArguments paging,
        ISelection selection,
        IFilterContext filter,
        IExecutionsByPipelineIdDataLoader dataloader,
        CancellationToken cancellation = default)
    {
        return await dataloader
            .WithPagingArguments(paging)
            .Where(filter)
            .Select(selection)
            .LoadAsync(pipeline.Id, cancellation)
            .ToConnectionAsync();
    }

    [DataLoader]
    internal static Task<Dictionary<Guid, Pipeline>> GetPipelineByIdAsync(
        IReadOnlyList<Guid> keys,
        ISelectorBuilder selector,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .Pipelines
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .Select(x => x.Id, selector)
            .ToDictionaryAsync(x => x.Id, cancellation);
    }

    [DataLoader]
    internal static ValueTask<Dictionary<Guid, Page<PipelineExecution>>> GetExecutionsByPipelineIdAsync(
        IReadOnlyList<Guid> keys,
        PagingArguments paging,
        IPredicateBuilder predicate,
        ISelectorBuilder selector,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .PipelineExecutions
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .Where(predicate)
            // .Select(x => x.Id, selector)
            .OrderBy(x => x.Id)
            .ToBatchPageAsync(x => x.Id, paging, cancellation);
    }
}