using System.Collections.Concurrent;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Engine responsible for orchestrating the parallel execution of pipeline stages while respecting dependency
/// constraints defined in the pipeline definition.
/// </summary>
internal sealed class PipelineEngine : IPipelineEngine
{
    // TODO: Make this configurable via options pattern for better scalability
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

            // initialize ready queue with stages that have no dependencies (root nodes)
            var ready = new ConcurrentQueue<PipelineStageDefinition>(graph.Value.GetRoots());

            // track remaining dependencies for each stage to determine when they become ready
            var pending = new ConcurrentDictionary<PipelineStageDefinition, int>();
            foreach (var stage in graph.Value.Nodes)
            {
                pending[stage] = graph.Value.GetParents(stage).Count();
            }

            // track currently executing tasks and their associated stages for completion handling
            var running = new ConcurrentDictionary<Task<ErrorOr<Success>>, PipelineStageDefinition>();

            // control parallelism to prevent resource exhaustion
            // TODO: Consider making this configurable based on system resources
            using var semaphore = new SemaphoreSlim(MaxParallelism);

            // main execution loop: process stages until all are complete
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

                // exit if no tasks are running and no stages are ready
                if (running.IsEmpty && ready.IsEmpty)
                    break;

                // wait for any running task to complete
                var completed = await Task.WhenAny(running.Keys);
                if (running.TryRemove(completed, out var executed))
                {
                    var result = await completed;
                    if (result.IsError)
                        return result.Errors;

                    // update dependencies for child stages and enqueue newly ready stages
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
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "unexpected error occurred in pipeline engine. error: {Error}", ex.Message);
            return Error.Unexpected(description: $"unexpected error occurred in pipeline engine. error: {ex.Message}");
        }
    }
}
