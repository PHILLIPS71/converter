using Giantnodes.Infrastructure;
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
    public static async Task<PipelineExecution?> GetPipelineExecutionByIdAsync(
        Id id,
        QueryContext<PipelineExecution> query,
        IPipelineExecutionByIdDataLoader dataloader,
        CancellationToken cancellation)
        => await dataloader
            .With(query)
            .LoadAsync(id, cancellation);

    [DataLoader]
    internal static async Task<Dictionary<Id, PipelineExecution>> GetPipelineExecutionByIdAsync(
        IReadOnlyList<Id> keys,
        QueryContext<PipelineExecution> query,
        ApplicationDbContext database,
        CancellationToken cancellation = default)
        => await database
            .PipelineExecutions
            .AsNoTracking()
            .Where(x => keys.Contains(x.Id))
            .With(query)
            .ToDictionaryAsync(x => x.Id, cancellation);
}
