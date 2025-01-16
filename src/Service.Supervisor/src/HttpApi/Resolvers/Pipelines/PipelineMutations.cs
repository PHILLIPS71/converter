using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.HttpApi.Types.Pipelines.Payloads;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Pipelines;

[MutationType]
internal sealed class PipelineMutations
{
    [Error<DomainException>]
    [Error<ValidationException>]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<Pipeline>> PipelineCreate(
        [Service] ApplicationDbContext database,
        [Service] IRequestClient<PipelineCreate.Command> request,
        PipelineCreate.Command input,
        CancellationToken cancellation = default)
    {
        Response response = await request.GetResponse<PipelineCreate.Result, DomainFault, ValidationFault>(input, cancellation);
        return response switch
        {
            (_, PipelineCreate.Result result) =>
                database.Pipelines.AsNoTracking().Where(x => x.Id == result.PipelineId),
            (_, DomainFault fault) => throw new DomainException(fault),
            (_, ValidationFault fault) => throw new ValidationException(fault),
            _ => throw new InvalidOperationException()
        };
    }

    [Error<DomainException>]
    [Error<ValidationException>]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<Pipeline>> PipelineUpdate(
        [Service] ApplicationDbContext database,
        [Service] IRequestClient<PipelineUpdate.Command> request,
        PipelineUpdate.Command input,
        CancellationToken cancellation = default)
    {
        Response response = await request.GetResponse<PipelineUpdate.Result, DomainFault, ValidationFault>(input, cancellation);
        return response switch
        {
            (_, PipelineUpdate.Result result) =>
                database.Pipelines.AsNoTracking().Where(x => x.Id == result.PipelineId),
            (_, DomainFault fault) => throw new DomainException(fault),
            (_, ValidationFault fault) => throw new ValidationException(fault),
            _ => throw new InvalidOperationException()
        };
    }

    [Error<DomainException>]
    [Error<ValidationException>]
    public async Task<PipelineExecutePayload> PipelineExecute(
        [Service] ApplicationDbContext database,
        [Service] IRequestClient<PipelineExecute.Command> request,
        PipelineExecute.Command input,
        CancellationToken cancellation = default)
    {
        Response response = await request.GetResponse<PipelineExecute.Result, DomainFault, ValidationFault>(input, cancellation);
        switch (response)
        {
            case (_, PipelineExecute.Result result):
            {
                var files = await database
                    .Files
                    .AsNoTracking()
                    .Where(x => result.Executions.Select(y => y.FileId).Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x, cancellation);

                var executions = await database
                    .PipelineExecutions
                    .AsNoTracking()
                    .Where(x => result.Executions.Select(y => y.PipelineExecutionId).Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x, cancellation);

                var items = result.Executions
                    .Select(item =>
                    {
                        var file = files.GetValueOrDefault(item.FileId);
                        if (file == null)
                            throw new InvalidOperationException($"file cannot be found in result: {item.FileId}");

                        var execution = item.PipelineExecutionId.HasValue
                            ? executions.GetValueOrDefault(item.PipelineExecutionId.Value)
                            : null;

                        return new PipelineExecutePayload.Result
                        {
                            File = file,
                            Execution = execution,
                            Faults = item.Faults
                        };
                    })
                    .ToList();

                return new PipelineExecutePayload { Results = items };
            }

            case (_, DomainFault fault):
                throw new DomainException(fault);

            case (_, ValidationFault fault):
                throw new ValidationException(fault);

            default:
                throw new InvalidOperationException();
        }
    }
}