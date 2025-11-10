using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Infrastructure.Pipelines.MassTransit;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Persistence.Configurations;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines;

public partial class PipelineFailedActivity : IStateMachineActivity<PipelineLifecycleSagaState, PipelineFailedEvent>
{
    private readonly IPipelineRepository _pipelines;

    public PipelineFailedActivity(IPipelineRepository pipelines)
    {
        _pipelines = pipelines;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(KebabCaseEndpointNameFormatter.Instance.Message<PipelineFailedActivity>());
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    [UnitOfWork]
    public async Task Execute(
        BehaviorContext<PipelineLifecycleSagaState, PipelineFailedEvent> context,
        IBehavior<PipelineLifecycleSagaState, PipelineFailedEvent> next)
    {
        var id = context.Message.Context
            .State
            .Get<string>("pipeline_execution_id")
            .Then(Id.Parse);

        if (id.IsError)
        {
            await context.RejectAsync(id.ToFaultKind(), id.ToFault());
            return;
        }

        var pipeline = await _pipelines.GetByPipelineExecutionIdAsync(id.Value, context.CancellationToken);
        if (pipeline == null)
        {
            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(id.Value));
            return;
        }

        var execution = pipeline.Executions.SingleOrDefault(x => x.Id == id);
        if (execution == null)
        {
            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(id.Value));
            return;
        }

        var result = execution.Fail(context.Message.Exceptions.Message, context.Message.RaisedAt);
        if (result.IsError)
            await context.RejectAsync(result.ToFaultKind(), result.ToFault());

        await next.Execute(context);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<PipelineLifecycleSagaState, PipelineFailedEvent, TException> context,
        IBehavior<PipelineLifecycleSagaState, PipelineFailedEvent> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }
}
