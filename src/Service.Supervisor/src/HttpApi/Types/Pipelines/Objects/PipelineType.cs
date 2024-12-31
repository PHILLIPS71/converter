using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
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
}