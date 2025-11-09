using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;

public interface IFileRepository : IRepository<FileSystemFile, Id>
{
}
