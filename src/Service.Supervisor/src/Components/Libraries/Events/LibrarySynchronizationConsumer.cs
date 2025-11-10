using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Libraries.Events;

public sealed partial class LibrarySynchronizationConsumer : IConsumer<LibraryFileSystemChangedEvent>
{
    private readonly IDirectoryScanningService _scanner;
    private readonly ILibraryRepository _libraries;

    public LibrarySynchronizationConsumer(
        IDirectoryScanningService scanner,
        ILibraryRepository libraries)
    {
        _scanner = scanner;
        _libraries = libraries;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<LibraryFileSystemChangedEvent> context)
    {
        var library = await _libraries.FindByIdAsync(context.Message.LibraryId, context.CancellationToken);
        if (library == null)
        {
            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(context.Message.LibraryId));
            return;
        }

        var result = await _scanner.TryScanDirectoryAsync(library.DirectoryId, context.CancellationToken);
        if (result.IsError)
            await context.RejectAsync(result.ToFaultKind(), result.ToFault());
    }
}
