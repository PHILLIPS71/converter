using ErrorOr;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public interface ILibraryService : IDomainService
{
    /// <summary>
    /// Creates a new library with the specified name at the given file system path.
    /// </summary>
    /// <param name="name">The unique name for the library.</param>
    /// <param name="path">The file system path where the library is located.</param>
    /// <param name="cancellation">Optional cancellation token.</param>
    /// <returns>The created library if successful; otherwise, an error.</returns>
    Task<ErrorOr<Library>> CreateAsync(LibraryName name, string path, CancellationToken cancellation = default);
}