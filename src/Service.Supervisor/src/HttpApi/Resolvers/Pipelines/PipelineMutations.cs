using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
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
            (_, PipelineCreate.Result result) => database.Pipelines.AsNoTracking().Where(x => x.Id == result.PipelineId),
            (_, DomainFault fault) => throw new DomainException(fault),
            (_, ValidationFault fault) => throw new ValidationException(fault),
            _ => throw new InvalidOperationException()
        };
    }
}