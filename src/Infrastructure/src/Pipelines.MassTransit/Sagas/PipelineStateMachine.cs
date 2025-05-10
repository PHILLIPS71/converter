using System.Collections.Concurrent;
using MassTransit;
using MassTransit.Contracts.JobService;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

internal sealed class PipelineStateMachine : MassTransitStateMachine<PipelineSagaState>
{
    public PipelineStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Submitted, e => e.CorrelateById(context => context.Message.CorrelationId));

        Event(() => StageCompleted,
            e => e.CorrelateBy((saga, context) => saga.Executing.ContainsValue(context.Message.JobId)));
        Event(() => StageFaulted,
            e => e.CorrelateBy((saga, context) => saga.Executing.ContainsValue(context.Message.JobId)));

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
            When(StageCompleted)
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
            When(StageCancelled)
                .Finalize(),
            When(StageFaulted)
                .Finalize());

        SetCompletedWhenFinalized();
    }

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public State Executing { get; }

    public Event<PipelineExecute.Command> Submitted { get; }
    public Event<JobCompleted<PipelineStageCompletedEvent>> StageCompleted { get; }
    public Event<JobFaulted> StageFaulted { get; }

    public Event<JobCanceled> StageCancelled { get; }
}

internal static class PipelineStateMachineBehaviorExtensions
{
    public static EventActivityBinder<PipelineSagaState, TEvent> BuildAsync<TEvent>(
        this EventActivityBinder<PipelineSagaState, TEvent> binder)
        where TEvent : class
    {
        return binder
            .ThenAsync(async context =>
            {
                var graph = context.Saga.Pipeline.ToGraph();
                if (graph.IsError)
                    throw new InvalidOperationException();

                if (graph.Value.IsEmpty())
                    throw new InvalidOperationException();

                var pending = new Dictionary<string, int>();
                foreach (var stage in graph.Value.Nodes)
                {
                    pending[stage.Id] = graph.Value.GetParents(stage).Count();
                }

                context.Saga.Pending = pending;

                foreach (var stage in graph.Value.GetRoots())
                {
                    await SubmitStageAsync(context, stage);
                }
            });
    }

    public static EventActivityBinder<PipelineSagaState, JobCompleted<PipelineStageCompletedEvent>> CompleteStageAsync(
        this EventActivityBinder<PipelineSagaState, JobCompleted<PipelineStageCompletedEvent>> binder)
    {
        return binder
            .ThenAsync(async context =>
            {
                var graph = context.Saga.Pipeline.ToGraph();
                if (graph.IsError)
                    throw new InvalidOperationException();

                if (graph.Value.IsEmpty())
                    throw new InvalidOperationException();

                var ready = new List<PipelineStageDefinition>();
                foreach (var child in graph.Value.GetChildren(context.Message.Job.Stage))
                {
                    var remaining = context.Saga.Pending[child.Id] -= 1;
                    if (remaining == 0)
                        ready.Add(child);
                }

                foreach (var stage in ready)
                {
                    await SubmitStageAsync(context, stage);
                }
            });
    }

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