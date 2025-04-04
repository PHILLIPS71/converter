using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Objects;

[ObjectType<PipelineExecution>]
public static partial class PipelineExecutionType
{
    static partial void Configure(IObjectTypeDescriptor<PipelineExecution> descriptor)
    {
        descriptor
            .Field(f => f.Id)
            .ID();

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
        QueryContext<PipelineExecution> query,
        IPipelineExecutionByIdDataLoader dataloader,
        CancellationToken cancellation)
    {
        return dataloader.With(query).LoadAsync(id, cancellation);
    }

    [DataLoader]
    internal static Task<Dictionary<Guid, PipelineExecution>> GetPipelineExecutionByIdAsync(
        IReadOnlyList<Guid> keys,
        QueryContext<PipelineExecution> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
    {
        return database
            .PipelineExecutions
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query, x => x.AddDescending(y => y.CreatedAt))
            .ToDictionaryAsync(x => x.Id, cancellation);
    }
}