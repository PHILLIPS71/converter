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
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        var running = new Dictionary<Task<ErrorOr<Success>>, PipelineStageDefinition>();

        try
        {
            var graph = definition.ToGraph();
            if (graph.IsError)
                return graph.Errors;

            if (graph.Value.IsEmpty())
                return Result.Success;

            // initialize execution state: ready queue with root stages, pending tracks remaining dependencies
            var ready = new Queue<PipelineStageDefinition>(graph.Value.GetRoots());
            var pending = graph.Value.Nodes.ToDictionary(
                stage => stage,
                stage => graph.Value.GetParents(stage).Count());

            // main execution loop: process stages until all complete or cancellation occurs
            while (ready.Count > 0 || running.Count > 0)
            {
                // fail-fast: exit immediately on cancellation
                cts.Token.ThrowIfCancellationRequested();

                // schedule ready stages up to parallelism limit
                while (ready.Count > 0 && running.Count < MaxParallelism)
                {
                    var stage = ready.Dequeue();
                    running.Add(_engine.ExecuteAsync(context, stage, cts.Token), stage);
                }

                if (running.Count == 0)
                    break;

                // wait for any running stage to complete
                var completed = await Task.WhenAny(running.Keys);
                var executed = running[completed];
                running.Remove(completed);

                var result = await completed;

                // on stage failure, cancel all other stages and return error
                if (result.IsError)
                {
                    await cts.CancelAsync();
                    return result.Errors;
                }

                // decrement dependencies and enqueue newly ready stages
                foreach (var child in graph.Value.GetChildren(executed))
                {
                    if (--pending[child] == 0)
                        ready.Enqueue(child);
                }
            }

            return Result.Success;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("pipeline {Id} ({Name}) execution was cancelled", context.Id, definition.Name);
            throw;
        }
        catch (Exception ex)
        {
            // unexpected errors: cancel running stages and return error
            await cts.CancelAsync();

            _logger.LogError(ex, "unexpected error occurred in pipeline {Id} ({Name})", context.Id, definition.Name);
            return Error.Unexpected(description: $"unexpected error occurred in pipeline engine. error: {ex.Message}");
        }
        finally
        {
            // ensure all running tasks complete before returning (prevents resource leaks)
            if (running.Count > 0)
                await Task.WhenAll(running.Keys);
        }
    }
}
