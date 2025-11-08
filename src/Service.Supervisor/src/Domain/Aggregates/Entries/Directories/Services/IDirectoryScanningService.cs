using ErrorOr;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

public interface IDirectoryScanningService : IDomainService
{
    Task<ErrorOr<Success>> TryScanDirectoryAsync(Guid directoryId, CancellationToken cancellation);
}
