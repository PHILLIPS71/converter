using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Selectors;
using HotChocolate.Execution.Processing;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Objects;

[ObjectType<PipelineExecution>]
public static partial class PipelineExecutionType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineExecution> descriptor)
    {
        descriptor
            .Field(f => f.Id);

        descriptor
            .Field(f => f.Pipeline);

        descriptor
            .Field(f => f.Definition);

        descriptor
            .Field(f => f.File);

        descriptor
            .Field(f => f.Status);

        descriptor
            .Field(f => f.Duration);

        descriptor
            .Field(f => f.StartedAt);

        descriptor
            .Field(f => f.Failure);

        descriptor
            .Field(f => f.CompletedAt);

        descriptor
            .Field(f => f.CreatedAt);

        descriptor
            .Field(f => f.UpdatedAt);
    }

    [NodeResolver]
    internal static Task<PipelineExecution?> GetPipelineExecutionByIdAsync(
        Guid id,
        ISelection selection,
        IPipelineExecutionByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.Select(selection).LoadAsync(id, cancellation);
    }

    [DataLoader]
    internal static Task<Dictionary<Guid, PipelineExecution>> GetPipelineExecutionByIdAsync(
        IReadOnlyList<Guid> keys,
        ISelectorBuilder selector,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .PipelineExecutions
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .Select(x => x.Id, selector)
            .ToDictionaryAsync(x => x.Id, cancellation);
    }
}