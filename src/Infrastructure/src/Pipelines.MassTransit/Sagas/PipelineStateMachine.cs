﻿using MassTransit;
using MassTransit.Contracts.JobService;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

internal sealed class PipelineStateMachine : MassTransitStateMachine<PipelineSagaState>
{
    public PipelineStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Submitted);
        Event(() => Completed, e => e.CorrelateBy((saga, context) => saga.Stages.Any(x => x.JobId == context.Message.JobId)));
        Event(() => Cancelled, e => e.CorrelateBy((saga, context) => saga.Stages.Any(x => x.JobId == context.Message.JobId)));
        Event(() => Faulted, e => e.CorrelateBy((saga, context) => saga.Stages.Any(x => x.JobId == context.Message.JobId)));

        Initially(
            When(Submitted)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.Pipeline = context.Message.Pipeline;
                    context.Saga.Context = context.Message.Context;
                })
                .StartAsync()
                .IfElse(context => context.Saga.Pipeline.Stages.Values.Count != 0,
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
                .IfElse(context => context.Saga.Stages.All(x => x.IsCompleted()),
                    complete => complete
                        .CompleteAsync()
                        .Finalize(),
                    incomplete => incomplete
                        .CompleteStageAsync()
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
                context.Saga.Stages = graph.Value.Nodes
                    .Select(stage => new PipelineStageSagaState
                    {
                        Stage = stage,
                        Dependencies = graph.Value.GetParents(stage).Count()
                    })
                    .ToList();

                // submit all root stages (stages with no dependencies) for immediate execution
                foreach (var stage in context.Saga.Stages.Where(x => x.Dependencies == 0))
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
                var completed = context.Saga.Stages.Single(x => x.JobId == context.Message.JobId);
                completed.CompletedAt = context.Message.Timestamp;

                var graph = context.Saga.Pipeline.ToGraph();
                if (graph.IsError)
                    throw new InvalidOperationException(
                        $"failed to rebuild pipeline graph for '{context.Saga.Pipeline.Name}' after stage '{completed.Stage.Name}' completion: {graph.FirstError.Description}");

                if (graph.Value.IsEmpty())
                    throw new InvalidOperationException(
                        $"pipeline '{context.Saga.Pipeline.Name}' graph is empty during stage '{completed.Stage.Name}' completion processing");

                // find child stages that are now ready to execute
                var ready = new List<PipelineStageSagaState>();
                foreach (var child in graph.Value.GetChildren(completed.Stage))
                {
                    var stage = context.Saga.Stages.Single(x => x.Stage.CorrelationId == child.CorrelationId);
                    stage.Dependencies -= 1;

                    if (stage.Dependencies == 0)
                        ready.Add(stage);
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
                var tasks = context.Saga.Stages
                    .Where(x => !x.IsCompleted())
                    .Where(x => x.JobId.HasValue &&
                                x.JobId.Value != context.Message.JobId) // don't cancel the already cancelled job
                    .Select(x => context.CancelJob(x.JobId!.Value))
                    .ToArray();

                if (tasks.Length > 0)
                    await Task.WhenAll(tasks);
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
                var tasks = context.Saga.Stages
                    .Where(x => !x.IsCompleted())
                    .Where(x => x.JobId.HasValue &&
                                x.JobId.Value != context.Message.JobId) // don't cancel the already faulted job
                    .Select(x => context.CancelJob(x.JobId!.Value))
                    .ToArray();

                if (tasks.Length > 0)
                    await Task.WhenAll(tasks);
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
    /// <param name="state">The stage saga state used to track stage execution.</param>
    /// <returns>A task representing the asynchronous job submission operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when job submission fails or when the stage is already being executed.</exception>
    private static async Task SubmitStageAsync<T>(
        this BehaviorContext<PipelineSagaState, T> context,
        PipelineStageSagaState state)
        where T : class
    {
        // check if stage is already executing to prevent duplicate submissions
        if (state.JobId.HasValue)
            throw new InvalidOperationException(
                $"stage '{state.Stage.Name}' is already executing in pipeline '{context.Saga.Pipeline.Name}'");

        var command = new PipelineStageExecute.Command
        {
            CorrelationId = context.Saga.CorrelationId,
            Pipeline = context.Saga.Pipeline,
            Stage = state.Stage,
            Context = context.Saga.Context
        };

        // submit the stage as a background job and track the job id
        state.JobId = await context.SubmitJob(state.Stage.CorrelationId, command, null, context.CancellationToken);
        state.StartedAt = DateTime.UtcNow;
    }
}