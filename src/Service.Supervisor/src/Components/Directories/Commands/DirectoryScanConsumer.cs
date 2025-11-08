using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Directories.Commands;

public sealed partial class DirectoryScanConsumer : IConsumer<DirectoryScan.Command>
{
    private readonly IDirectoryScanningService _scanner;

    public DirectoryScanConsumer(IDirectoryScanningService scanner)
    {
        _scanner = scanner;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<DirectoryScan.Command> context)
    {
        var result = await _scanner.TryScanDirectoryAsync(context.Message.DirectoryId, context.CancellationToken);
        if (result.IsError)
        {
            await context.RejectAsync(result.ToFaultKind(), result.ToFault());
            return;
        }

        await context.RespondAsync(new DirectoryScan.Result { DirectoryId = context.Message.DirectoryId });
    }
}
