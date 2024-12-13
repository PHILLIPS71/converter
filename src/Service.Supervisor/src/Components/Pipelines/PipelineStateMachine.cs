using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.Sagas;
using MassTransit;
using MassTransit.Contracts.JobService;

namespace Giantnodes.Service.Supervisor.Components.Pipelines;

internal sealed class PipelineStateMachine : MassTransitStateMachine<PipelineSagsState>
{
    public PipelineStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Submitted);
        Event(() => Executed, e => e.CorrelateBy((instance, context) => instance.JobId == context.Message.JobId));
        Event(() => Faulted, e => e.CorrelateBy((instance, context) => instance.JobId == context.Message.JobId));

        Initially(
            When(Submitted)
                .Then(context => context.Saga.Definition = context.Message.Definition)
                .IfElse(context => context.Saga.Definition.Specifications.Count != 0,
                    incomplete => incomplete
                        .ExecuteSpecificationAsync()
                        .TransitionTo(Executing),
                    complete => complete
                        .Finalize()
                )
        );

        During(Executing,
            When(Executed)
                .Then(context => context.Saga.Specification += 1)
                .IfElse(context => context.Saga.Specification < context.Saga.Definition.Specifications.Count,
                    incomplete => incomplete.ExecuteSpecificationAsync(),
                    complete => complete.Finalize()
                ),
            When(Faulted)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public State Executing { get; }

    public Event<PipelineStartedEvent> Submitted { get; }
    public Event<JobCompleted<PipelineSpecificationExecute.Command>> Executed { get; }
    public Event<JobFaulted> Faulted { get; }
}

internal static class PipelineStateMachineBehaviorExtensions
{
    public static EventActivityBinder<PipelineSagsState, TEvent> ExecuteSpecificationAsync<TEvent>(
        this EventActivityBinder<PipelineSagsState, TEvent> binder)
        where TEvent : class
    {
        return binder
            .ThenAsync(async context =>
            {
                var command = new PipelineSpecificationExecute.Command
                {
                    Specification = context.Saga.Definition.Specifications.ElementAt(context.Saga.Specification)
                };

                context.Saga.JobId = await context.SubmitJob(command, context.CancellationToken);
            });
    }
}