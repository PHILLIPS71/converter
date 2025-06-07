using MassTransit;
using MassTransit.Contracts.JobService;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

/// <summary>
/// State machine that orchestrates the execution of pipeline stages using MassTransit's job service.
/// Manages the lifecycle of pipeline execution from submission through completion, handling dependencies
/// between stages and coordinating parallel execution of independent stages.
/// </summary>
internal sealed class PipelineStateMachine : MassTransitStateMachine<PipelineSagaState>
{
    public PipelineStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Submitted);
        Event(() => Completed, e => e.CorrelateBy((saga, context) => saga.Executing.ContainsValue(context.Message.JobId)));
        Event(() => Cancelled, e => e.CorrelateBy((saga, context) => saga.Executing.ContainsValue(context.Message.JobId)));
        Event(() => Faulted, e => e.CorrelateBy((saga, context) => saga.Executing.ContainsValue(context.Message.JobId)));

        Initially(
            When(Submitted)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.Pipeline = context.Message.Pipeline;
                    context.Saga.Context = context.Message.Context;
                })
                .StartAsync()
                .IfElse(context => context.Saga.Pipeline.Stages.Count != 0,
                    incomplete => incomplete
                        .BuildAsync()
                        .TransitionTo(Executing),
                    complete => complete
                        .CompleteAsync()
                        .Finalize()
                )
        );

        During(Executing,
            When(Completed)
                .Then(context => context.Saga.Completed.Add(context.Message.Job.Stage.Id))
                .IfElse(context => context.Saga.Completed.Count < context.Saga.Pipeline.Stages.Count,
                    incomplete => incomplete
                        .CompleteStageAsync(),
                    complete => complete
                        .CompleteAsync()
                        .Finalize()
                )
        );

        DuringAny(
            When(Cancelled)
                .CancelAsync()
                .Finalize(),
            When(Faulted)
                .FailAsync()
                .Finalize());

        SetCompletedWhenFinalized();
    }

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public State Executing { get; }

    public Event<PipelineExecute.Command> Submitted { get; }
    public Event<JobCompleted<PipelineStageCompletedEvent>> Completed { get; }
    public Event<JobFaulted> Faulted { get; }
    public Event<JobCanceled> Cancelled { get; }
}

internal static class PipelineStateMachineBehaviorExtensions
{
    /// <summary>
    /// Builds the pipeline execution graph and submits root stages for processing.
    /// </summary>
    public static EventActivityBinder<PipelineSagaState, TEvent> BuildAsync<TEvent>(
        this EventActivityBinder<PipelineSagaState, TEvent> binder)
        where TEvent : class
    {
        return binder
            .ThenAsync(async context =>
            {
                var graph = context.Saga.Pipeline.ToGraph();
                if (graph.IsError)
                    throw new InvalidOperationException(
                        $"failed to build pipeline graph for '{context.Saga.Pipeline.Name}': {graph.FirstError.Description}");

                if (graph.Value.IsEmpty())
                    throw new InvalidOperationException(
                        $"pipeline '{context.Saga.Pipeline.Name}' contains no stages to execute");

                // initialize dependency tracking for all stages
                var pending = new Dictionary<string, int>();
                foreach (var stage in graph.Value.Nodes)
                {
                    pending[stage.Id] = graph.Value.GetParents(stage).Count();
                }

                context.Saga.Pending = pending;

                // submit all root stages (stages with no dependencies) for immediate execution
                foreach (var stage in graph.Value.GetRoots())
                {
                    await SubmitStageAsync(context, stage);
                }
            });
    }

    /// <summary>
    /// Processes stage completion and submit newly ready dependent stages for execution.
    /// </summary>
    public static EventActivityBinder<PipelineSagaState, JobCompleted<PipelineStageCompletedEvent>> CompleteStageAsync(
        this EventActivityBinder<PipelineSagaState, JobCompleted<PipelineStageCompletedEvent>> binder)
    {
        return binder
            .ThenAsync(async context =>
            {
                var completed = context.Message.Job.Stage;

                // remove the completed stage from the executing list as it's no longer executing
                context.Saga.Executing.Remove(completed.Id);

                var graph = context.Saga.Pipeline.ToGraph();
                if (graph.IsError)
                    throw new InvalidOperationException(
                        $"failed to rebuild pipeline graph for '{context.Saga.Pipeline.Name}' after stage '{completed.Id}' completion: {graph.FirstError.Description}");

                if (graph.Value.IsEmpty())
                    throw new InvalidOperationException(
                        $"pipeline '{context.Saga.Pipeline.Name}' graph is empty during stage '{completed.Id}' completion processing");

                // find child stages that are now ready to execute
                var ready = new List<PipelineStageDefinition>();
                foreach (var child in graph.Value.GetChildren(completed))
                {
                    // decrement dependency count and check if stage is ready
                    var remaining = context.Saga.Pending[child.Id] -= 1;
                    if (remaining == 0)
                        ready.Add(child);
                }

                // submit all newly ready stages for execution
                foreach (var stage in ready)
                {
                    await SubmitStageAsync(context, stage);
                }
            });
    }

    /// <summary>
    /// Publishes the pipeline started event to notify subscribers that execution has begun.
    /// </summary>
    public static EventActivityBinder<PipelineSagaState, PipelineExecute.Command> StartAsync(
        this EventActivityBinder<PipelineSagaState, PipelineExecute.Command> binder)
    {
        return binder
            .PublishAsync(context => context.Init<PipelineStartedEvent>(new PipelineStartedEvent
            {
                CorrelationId = context.Saga.CorrelationId,
                Pipeline = context.Saga.Pipeline,
                Context = context.Saga.Context,
            }));
    }

    /// <summary>
    /// Publishes the pipeline completed event to notify subscribers that all stages have finished successfully.
    /// </summary>
    public static EventActivityBinder<PipelineSagaState, TEvent> CompleteAsync<TEvent>(
        this EventActivityBinder<PipelineSagaState, TEvent> binder)
        where TEvent : class
    {
        return binder
            .PublishAsync(context => context.Init<PipelineCompletedEvent>(new PipelineCompletedEvent
            {
                CorrelationId = context.Saga.CorrelationId,
                Pipeline = context.Saga.Pipeline,
                Context = context.Saga.Context,
            }));
    }

    /// <summary>
    /// Handles pipeline cancellation by cancelling all remaining jobs and publishing a cancellation event. Uses
    /// all-or-nothing semantics, if any stage is cancelled, the entire pipeline is cancelled.
    /// </summary>
    public static EventActivityBinder<PipelineSagaState, JobCanceled> CancelAsync(
        this EventActivityBinder<PipelineSagaState, JobCanceled> binder)
    {
        return binder
            .ThenAsync(async context =>
            {
                // cancel ALL remaining executing jobs, all or nothing approach
                var tasks = context.Saga.Executing.Values
                    .Where(id => id != context.Message.JobId) // don't cancel the already cancelled job
                    .Select(id => context.CancelJob(id))
                    .ToArray();

                if (tasks.Length > 0)
                    await Task.WhenAll(tasks);

                // clear all tracking since an entire pipeline is cancelled
                context.Saga.Executing.Clear();
                context.Saga.Pending.Clear();
                context.Saga.Completed.Clear();
            })
            .PublishAsync(context => context.Init<PipelineCancelledEvent>(new PipelineCancelledEvent
            {
                CorrelationId = context.Saga.CorrelationId,
                Pipeline = context.Saga.Pipeline,
                Context = context.Saga.Context
            }));
    }

    /// <summary>
    /// Handles pipeline failure by cancelling all remaining jobs and publishing a failure event. Uses all-or-nothing
    /// semantics, if any stage fails, the entire pipeline fails.
    /// </summary>
    public static EventActivityBinder<PipelineSagaState, JobFaulted> FailAsync(
        this EventActivityBinder<PipelineSagaState, JobFaulted> binder)
    {
        return binder
            .ThenAsync(async context =>
            {
                // cancel ALL remaining executing jobs, all or nothing approach for video processing
                var tasks = context.Saga.Executing.Values
                    .Where(id => id != context.Message.JobId) // don't cancel the already faulted job
                    .Select(id => context.CancelJob(id))
                    .ToArray();

                if (tasks.Length > 0)
                    await Task.WhenAll(tasks);

                // clear all tracking since an entire pipeline has failed
                context.Saga.Executing.Clear();
                context.Saga.Pending.Clear();
                context.Saga.Completed.Clear();
            })
            .PublishAsync(context => context.Init<PipelineFailedEvent>(new PipelineFailedEvent
            {
                CorrelationId = context.Saga.CorrelationId,
                Pipeline = context.Saga.Pipeline,
                Context = context.Saga.Context,
                Exceptions = context.Message.Exceptions
            }));
    }

    /// <summary>
    /// Submits a pipeline stage for execution as a background job and tracks it in the saga state.
    /// </summary>
    /// <typeparam name="T">The type of the current event being processed.</typeparam>
    /// <param name="context">The behavior context containing the saga state.</param>
    /// <param name="stage">The stage definition to submit for execution.</param>
    /// <returns>A task representing the asynchronous job submission operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when job submission fails or when the stage is already being executed.</exception>
    private static async Task SubmitStageAsync<T>(
        this BehaviorContext<PipelineSagaState, T> context,
        PipelineStageDefinition stage)
        where T : class
    {
        // check if stage is already executing to prevent duplicate submissions
        if (context.Saga.Executing.ContainsKey(stage.Id))
            throw new InvalidOperationException(
                $"Stage '{stage.Id}' is already executing in pipeline '{context.Saga.Pipeline.Name}'");

        var command = new PipelineStageExecute.Command
        {
            CorrelationId = context.Saga.CorrelationId,
            Pipeline = context.Saga.Pipeline,
            Stage = stage,
            Context = context.Saga.Context
        };

        try
        {
            // submit the stage as a background job and track the job ID
            context.Saga.Executing[stage.Id] = await context.SubmitJob(command, context.CancellationToken);

            // clean up pending tracking since the stage is now executing
            if (context.Saga.Pending.TryGetValue(stage.Id, out var pending) && pending == 0)
                context.Saga.Pending.Remove(stage.Id);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"failed to submit stage '{stage.Id}' for execution in pipeline '{context.Saga.Pipeline.Name}': {ex.Message}",
                ex);
        }
    }
}