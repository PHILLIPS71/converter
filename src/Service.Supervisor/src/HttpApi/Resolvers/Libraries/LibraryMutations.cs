using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Libraries;

[MutationType]
internal sealed class LibraryMutations
{
    [Error<DomainException>]
    [Error<ValidationException>]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<Library>> LibraryCreate(
        [Service] ApplicationDbContext database,
        [Service] IRequestClient<LibraryCreate.Command> request,
        LibraryCreate.Command input,
        CancellationToken cancellation = default)
    {
        Response response = await request.GetResponse<LibraryCreate.Result, DomainFault, ValidationFault>(input, cancellation);
        return response switch
        {
            (_, LibraryCreate.Result result) => database.Libraries.AsNoTracking().Where(x => x.Id == result.LibraryId),
            (_, DomainFault fault) => throw new DomainException(fault),
            (_, ValidationFault fault) => throw new ValidationException(fault),
            _ => throw new InvalidOperationException()
        };
    }
}
