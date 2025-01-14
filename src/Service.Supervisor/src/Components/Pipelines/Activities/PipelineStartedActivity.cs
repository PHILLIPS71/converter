using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.Sagas;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines.Activities;

public sealed partial class PipelineStartedActivity : IStateMachineActivity<PipelineSagaState>
{
    private readonly IPipelineRepository _pipelines;

    public PipelineStartedActivity(IPipelineRepository pipelines)
    {
        _pipelines = pipelines;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(KebabCaseEndpointNameFormatter.Instance.Message<PipelineStartedActivity>());
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<PipelineSagaState> context, IBehavior<PipelineSagaState> next)
    {
        await Execute(context);
        await next.Execute(context);
    }

    public async Task Execute<T>(BehaviorContext<PipelineSagaState, T> context, IBehavior<PipelineSagaState, T> next)
        where T : class
    {
        await Execute(context);
        await next.Execute(context);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<PipelineSagaState, TException> context,
        IBehavior<PipelineSagaState> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }

    public Task Faulted<T, TException>(
        BehaviorExceptionContext<PipelineSagaState, T, TException> context,
        IBehavior<PipelineSagaState, T> next)
        where T : class
        where TException : Exception
    {
        return next.Faulted(context);
    }

    [UnitOfWork]
    private async Task Execute(SagaConsumeContext<PipelineSagaState> context)
    {
        var id = context.Saga.Context
            .Get<string>("__pipeline_execution_id")
            .Then(Guid.Parse);

        if (id.IsError)
        {
            await context.RejectAsync(id.ToFaultKind(), id.ToFault());
            return;
        }

        var pipeline = await _pipelines.GetByPipelineExecutionIdAsync(id.Value, context.CancellationToken);
        if (pipeline == null)
            throw new InvalidOperationException($"pipeline not found for execution id: {id.Value}");

        var execution = pipeline.Executions.SingleOrDefault(x => x.Id == id);
        if (execution == null)
            throw new InvalidOperationException($"pipeline execution {id.Value} not found in pipeline {pipeline.Id}");

        var result = execution.Start();
        if (result.IsError)
            await context.RejectAsync(result.ToFaultKind(), result.ToFault());
    }
}