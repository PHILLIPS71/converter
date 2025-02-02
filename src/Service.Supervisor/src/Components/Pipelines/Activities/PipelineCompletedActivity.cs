using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.Sagas;
using MassTransit;
using MassTransit.Contracts.JobService;

namespace Giantnodes.Service.Supervisor.Components.Pipelines.Activities;

public sealed partial class PipelineCompletedActivity : IStateMachineActivity<PipelineSagaState, JobCompleted>
{
    private readonly IPipelineRepository _pipelines;

    public PipelineCompletedActivity(IPipelineRepository pipelines)
    {
        _pipelines = pipelines;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(KebabCaseEndpointNameFormatter.Instance.Message<PipelineCompletedActivity>());
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    [UnitOfWork]
    public async Task Execute(
        BehaviorContext<PipelineSagaState, JobCompleted> context,
        IBehavior<PipelineSagaState, JobCompleted> next)
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

        var result = execution.Complete(context.Message.Timestamp);
        if (result.IsError)
        {
            await context.RejectAsync(result.ToFaultKind(), result.ToFault());
            return;
        }

        await next.Execute(context);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<PipelineSagaState, JobCompleted, TException> context,
        IBehavior<PipelineSagaState, JobCompleted> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }
}