using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public interface ILibraryService : IDomainService
{
    /// <summary>
    /// Creates a new library with the specified name at the given file system path.
    /// </summary>
    /// <param name="name">The unique name for the library.</param>
    /// <param name="slug">The unique slug for the library.</param>
    /// <param name="path">The file system path where the library is located.</param>
    /// <param name="cancellation">Optional cancellation token.</param>
    /// <returns>The created library if successful; otherwise, an error.</returns>
    Task<ErrorOr<Library>> CreateAsync(Name name, Slug slug, string path, CancellationToken cancellation = default);
}
