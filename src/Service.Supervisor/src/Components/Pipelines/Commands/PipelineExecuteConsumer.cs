using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Infrastructure.Services;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines;

public sealed partial class PipelineExecuteConsumer : IConsumer<PipelineExecute.Command>
{
    private readonly IPipelineRepository _pipelines;
    private readonly IFileSystemEntryRepository _entries;
    private readonly IDirectoryRepository _directories;
    private readonly IPipelineExecutionService _execution;

    public PipelineExecuteConsumer(
        IPipelineRepository pipelines,
        IFileSystemEntryRepository entries,
        IDirectoryRepository directories,
        IPipelineExecutionService execution)
    {
        _pipelines = pipelines;
        _entries = entries;
        _directories = directories;
        _execution = execution;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<PipelineExecute.Command> context)
    {
        var pipeline = await _pipelines.SingleOrDefaultAsync(new IdSpecification<Pipeline, Id>(context.Message.PipelineId), context.CancellationToken);
        if (pipeline == null)
        {
            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(context.Message.PipelineId));
            return;
        }

        var entries = await _entries.ToListAsync(new IdSpecification<FileSystemEntry, Id>(context.Message.Entries), context.CancellationToken);
        if (entries.Count != context.Message.Entries.Count)
        {
            var missing = context.Message.Entries
                .Except(entries.Select(x => x.Id))
                .ToList();

            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(missing));
            return;
        }

        var files = new HashSet<FileSystemFile>(new FileSystemFileIdComparer());
        foreach (var entry in entries)
        {
            var items = entry switch
            {
                FileSystemFile file => [file],
                FileSystemDirectory directory => await _directories.GetFiles(directory, context.CancellationToken),
                _ => []
            };

            files.UnionWith(items);
        }

        if (files.Count == 0)
        {
            await context.RejectAsync(FaultKind.Validation, FaultProperty.Create("no files found to process for the given entry"));
            return;
        }

        var results = await Task.WhenAll(files.Select(async item =>
        {
            var execution = await _execution.ExecuteAsync(pipeline, item, context.CancellationToken);

            return execution.IsError
                ? new PipelineExecute.Result.ExecutionResult(item.Id, null, execution.ToFault())
                : new PipelineExecute.Result.ExecutionResult(item.Id, execution.Value.Id);
        }));

        await context.RespondAsync(new PipelineExecute.Result { Executions = results });
    }

    private sealed class FileSystemFileIdComparer : IEqualityComparer<FileSystemFile>
    {
        public bool Equals(FileSystemFile? x, FileSystemFile? y) =>
            x?.Id == y?.Id;

        public int GetHashCode(FileSystemFile obj) =>
            obj.Id.GetHashCode();
    }
}
