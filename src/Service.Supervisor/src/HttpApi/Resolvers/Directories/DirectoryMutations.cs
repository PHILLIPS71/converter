using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Persistence.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Giantnodes.Service.Supervisor.HttpApi.Resolvers.Directories;

[MutationType]
internal sealed class DirectoryMutations
{
    [Error<DomainException>]
    [Error<ValidationException>]
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<FileSystemDirectory>> DirectoryScan(
        [Service] ApplicationDbContext database,
        [Service] IRequestClient<DirectoryScan.Command> request,
        [ID] Guid directoryId,
        CancellationToken cancellation = default)
    {
        var input = new DirectoryScan.Command
        {
            DirectoryId = directoryId,
        };

        Response response = await request.GetResponse<DirectoryScan.Result, DomainFault, ValidationFault>(input, cancellation);
        return response switch
        {
            (_, DirectoryScan.Result result) => database.Directories.AsNoTracking().Where(x => x.Id == result.DirectoryId),
            (_, DomainFault fault) => throw new DomainException(fault),
            (_, ValidationFault fault) => throw new ValidationException(fault),
            _ => throw new InvalidOperationException()
        };
    }
}