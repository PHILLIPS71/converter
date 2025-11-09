using ErrorOr;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

public interface IDirectoryScanningService : IDomainService
{
    public Task<ErrorOr<Success>> TryScanDirectoryAsync(Id id, CancellationToken cancellation);
}
