using System.Collections.Concurrent;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines;

/// <inheritdoc />
internal sealed class PipelineEngine : IPipelineEngine
{
    private const int MaxParallelism = 3;

    private readonly IPipelineStageEngine _engine;
    private readonly ILogger<PipelineEngine> _logger;

    public PipelineEngine(IPipelineStageEngine engine, ILogger<PipelineEngine> logger)
    {
        _engine = engine;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default)
    {
        try
        {
            var graph = definition.ToGraph();
            if (graph.IsError)
                return graph.Errors;

            if (graph.Value.IsEmpty())
                return Result.Success;

            // initialize with stages that have no dependencies
            var ready = new ConcurrentQueue<PipelineStageDefinition>(graph.Value.GetRoots());

            // track remaining dependencies for each stage
            var pending = new ConcurrentDictionary<PipelineStageDefinition, int>();
            foreach (var stage in graph.Value.Nodes)
            {
                pending[stage] = graph.Value.GetParents(stage).Count();
            }

            // track running tasks with their associated stages
            var running = new ConcurrentDictionary<Task<ErrorOr<Success>>, PipelineStageDefinition>();
            var semaphore = new SemaphoreSlim(MaxParallelism);

            // process stages until all are complete
            while (!ready.IsEmpty || !running.IsEmpty)
            {
                cancellation.ThrowIfCancellationRequested();

                // start executing ready stages up to the parallelism limit
                while (ready.TryDequeue(out var stage))
                {
                    await semaphore.WaitAsync(cancellation);

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var result = await _engine.ExecuteAsync(context, stage, cancellation);
                            if (result.IsError)
                                return result.Errors;

                            return result;
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, cancellation);

                    running.TryAdd(task, stage);
                }

                if (running.IsEmpty && ready.IsEmpty)
                    break;

                // wait for any running task to complete
                var completed = await Task.WhenAny(running.Keys);
                if (running.TryRemove(completed, out var executed))
                {
                    var result = await completed;
                    if (result.IsError)
                        return result.Errors;

                    // update dependencies for child stages
                    foreach (var child in graph.Value.GetChildren(executed))
                    {
                        var remaining = pending.AddOrUpdate(child, _ => 0, (_, dependencies) => dependencies - 1);
                        if (remaining == 0)
                            ready.Enqueue(child);
                    }
                }
            }

            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "unexpected error occurred in pipeline engine. error: {Error}", ex.Message);
            return Error.Unexpected(description: $"unexpected error occurred in pipeline engine. error: {ex.Message}");
        }
    }
}