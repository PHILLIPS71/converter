using Giantnodes.Infrastructure;
using Giantnodes.Service.Runner.Contracts.Probing.Jobs;
using Giantnodes.Service.Supervisor.Contracts.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Entries;

public sealed class FileSystemEntryProbeConsumer : IConsumer<FileSystemEntryProbe.Command>
{
    private readonly IFileSystemEntryRepository _entires;

    public FileSystemEntryProbeConsumer(IFileSystemEntryRepository entires)
    {
        _entires = entires;
    }

    public async Task Consume(ConsumeContext<FileSystemEntryProbe.Command> context)
    {
        var entry = await _entires.SingleOrDefaultAsync(new IdSpecification<FileSystemEntry, Id>(context.Message.EntryId), context.CancellationToken);
        if (entry == null)
        {
            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(context.Message.EntryId));
            return;
        }

        await context.SubmitJob(new FileSystemProbe.Job { Path = entry.PathInfo.FullName }, context.CancellationToken);
        await context.RespondAsync(new FileSystemEntryProbe.Result { EntryId = entry.Id });
    }
}
