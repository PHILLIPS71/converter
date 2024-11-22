using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Entries;

[MutationType]
internal sealed class EntryMutations
{
    [Error<DomainException>]
    [Error<ValidationException>]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<FileSystemEntry>> EntryProbe(
        [Service] ApplicationDbContext database,
        [Service] IRequestClient<FileSystemEntryProbe.Command> request,
        [ID] Guid entryId,
        CancellationToken cancellation = default)
    {
        var input = new FileSystemEntryProbe.Command
        {
            EntryId = entryId,
        };

        Response response = await request.GetResponse<FileSystemEntryProbe.Result, DomainFault, ValidationFault>(input, cancellation);
        return response switch
        {
            (_, FileSystemEntryProbe.Result result) => database.Entries.AsNoTracking().Where(x => x.Id == result.EntryId),
            (_, DomainFault fault) => throw new DomainException(fault),
            (_, ValidationFault fault) => throw new ValidationException(fault),
            _ => throw new InvalidOperationException()
        };
    }
}