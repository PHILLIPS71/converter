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
                .IfElse(context => context.Saga.Pipeline.Stages.Count != 0,
                    incomplete => incomplete
                        .BuildAsync()
                        .TransitionTo(Executing),
                    complete => complete
                        .CompleteAsync()
                        .Finalize()
                )
                .StartAsync()
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
                .PublishAsync(context => context.Init<PipelineCancelledEvent>(new PipelineCancelledEvent
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Pipeline = context.Saga.Pipeline,
                    Context = context.Saga.Context,
                }))
                .Finalize(),
            When(Faulted)
                .PublishAsync(context => context.Init<PipelineFailedEvent>(new PipelineFailedEvent
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Pipeline = context.Saga.Pipeline,
                    Context = context.Saga.Context,
                    Exceptions = context.Message.Exceptions
                }))
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
        var command = new PipelineStageExecute.Command
        {
            CorrelationId = context.Saga.CorrelationId,
            Pipeline = context.Saga.Pipeline,
            Stage = stage,
            Context = context.Saga.Context
        };

        context.Saga.Executing[stage.Id] = await context.SubmitJob(command, context.CancellationToken);
    }
}