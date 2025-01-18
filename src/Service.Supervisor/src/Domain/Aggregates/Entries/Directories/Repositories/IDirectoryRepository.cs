using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

public interface IDirectoryRepository : IRepository<FileSystemDirectory>
{
    Task<List<FileSystemFile>> GetFiles(
        FileSystemDirectory directory,
        CancellationToken cancellation = default);
}