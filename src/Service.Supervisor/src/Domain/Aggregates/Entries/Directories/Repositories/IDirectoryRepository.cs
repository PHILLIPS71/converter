using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

public interface IDirectoryRepository : IRepository<FileSystemDirectory>
{
    Task<List<FileSystemFile>> GetFiles(
        FileSystemDirectory directory,
        CancellationToken cancellation = default);

    /// <summary>
    /// Loads a directory and all its contents, including all nested files and folders.
    /// </summary>
    /// <param name="id">The ID of the directory to load.</param>
    /// <param name="cancellation">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The directory with all its contents loaded, or null if not found.</returns>
    Task<FileSystemDirectory?> GetDirectoryHierarchy(
        Guid id,
        CancellationToken cancellation = default);
}
