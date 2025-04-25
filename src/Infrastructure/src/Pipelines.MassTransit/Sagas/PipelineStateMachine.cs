using MassTransit;
using MassTransit.Contracts.JobService;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

internal sealed class PipelineStateMachine : MassTransitStateMachine<PipelineSagaState>
{
    public PipelineStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Submitted, e => e.CorrelateById(context => context.Message.MessageId));
        Event(() => Executed, e => e.CorrelateBy((instance, context) => instance.JobId == context.Message.JobId));
        Event(() => Canceled, e => e.CorrelateBy((instance, context) => instance.JobId == context.Message.JobId));
        Event(() => Faulted, e => e.CorrelateBy((instance, context) => instance.JobId == context.Message.JobId));

        Initially(
            When(Submitted)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.MessageId;
                    context.Saga.Definition = context.Message.Definition;
                    context.Saga.Context = new PipelineContext(context.Message.State);
                })
                .PublishAsync(context => context.Init<PipelineStartedEvent>(new PipelineStartedEvent
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Definition = context.Saga.Definition,
                    Context = context.Saga.Context,
                }))
                .IfElse(context => context.Saga.Definition.Specifications.Count != 0,
                    incomplete => incomplete
                        .ExecuteSpecificationAsync()
                        .TransitionTo(Executing),
                    complete => complete
                        .CompleteAsync()
                        .Finalize()
                )
        );

        During(Executing,
            When(Executed)
                .Then(context => context.Saga.Specification += 1)
                .IfElse(context => context.Saga.Specification < context.Saga.Definition.Specifications.Count,
                    incomplete => incomplete.ExecuteSpecificationAsync(),
                    complete => complete
                        .CompleteAsync()
                        .Finalize()
                )
        );

        DuringAny(
            When(Canceled)
                .Finalize(),
            When(Faulted)
                .PublishAsync(context => context.Init<PipelineFailedEvent>(new PipelineFailedEvent
                {
                    CorrelationId = context.Saga.CorrelationId,
                    Definition = context.Saga.Definition,
                    Context = context.Saga.Context,
                    Exceptions = context.Message.Exceptions
                }))
                .Finalize());

        SetCompletedWhenFinalized();
    }

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public State Executing { get; }

    public Event<PipelineExecute.Command> Submitted { get; }
    public Event<JobCompleted> Executed { get; }
    public Event<JobCanceled> Canceled { get; }
    public Event<JobFaulted> Faulted { get; }
}

internal static class PipelineStateMachineBehaviorExtensions
{
    public static EventActivityBinder<PipelineSagaState, TEvent> ExecuteSpecificationAsync<TEvent>(
        this EventActivityBinder<PipelineSagaState, TEvent> binder)
        where TEvent : class
    {
        return binder
            .ThenAsync(async context =>
            {
                var command = new PipelineSpecificationExecute.Job
                {
                    State = context.Saga.Context.State,
                    Specification = context.Saga.Definition.Specifications.ElementAt(context.Saga.Specification),
                };

                context.Saga.JobId = await context.SubmitJob(command, context.CancellationToken);
            });
    }

    public static EventActivityBinder<PipelineSagaState, TEvent> CompleteAsync<TEvent>(
        this EventActivityBinder<PipelineSagaState, TEvent> binder)
        where TEvent : class
    {
        return binder
            .PublishAsync(context => context.Init<PipelineCompletedEvent>(new PipelineCompletedEvent
            {
                CorrelationId = context.Saga.CorrelationId,
                Definition = context.Saga.Definition,
                Context = context.Saga.Context,
            }));
    }
}