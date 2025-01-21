using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Libraries.Events;

public sealed partial class NewLibraryScanConsumer : IConsumer<LibraryCreatedEvent>
{
    private readonly IDirectoryScanningService _scanner;

    public NewLibraryScanConsumer(IDirectoryScanningService scanner)
    {
        _scanner = scanner;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<LibraryCreatedEvent> context)
    {
        await _scanner.TryScanDirectoryAsync(context.Message.DirectoryId, context.CancellationToken);
    }
}