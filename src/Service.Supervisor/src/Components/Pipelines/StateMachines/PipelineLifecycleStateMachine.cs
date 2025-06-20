﻿using Giantnodes.Infrastructure.Pipelines.MassTransit;
using Giantnodes.Service.Supervisor.Persistence.Configurations;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines;

public sealed class PipelineLifecycleStateMachine : MassTransitStateMachine<PipelineLifecycleSagaState>
{
    public PipelineLifecycleStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => Started);
        Event(() => Completed);
        Event(() => Cancelled);
        Event(() => Failed);

        Initially(
            When(Started)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                })
                .Activity(context => context.OfType<PipelineStartedActivity>())
                .TransitionTo(Running)
        );

        During(Running,
            When(Completed)
                .Activity(context => context.OfType<PipelineCompletedActivity>())
                .Finalize(),
            When(Cancelled)
                .Activity(context => context.OfType<PipelineCancelledActivity>())
                .Finalize(),
            When(Failed)
                .Activity(context => context.OfType<PipelineFailedActivity>())
                .Finalize()
        );
    }

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public State Running { get; }

    public Event<PipelineStartedEvent> Started { get; }
    public Event<PipelineCompletedEvent> Completed { get; }
    public Event<PipelineCancelledEvent> Cancelled { get; }
    public Event<PipelineFailedEvent> Failed { get; }
}