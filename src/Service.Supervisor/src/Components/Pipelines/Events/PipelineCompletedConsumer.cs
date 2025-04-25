using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.Pipelines.MassTransit;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines.Events;

public sealed partial class PipelineCompletedConsumer : IConsumer<PipelineCompletedEvent>
{
    private readonly IPipelineRepository _pipelines;

    public PipelineCompletedConsumer(IPipelineRepository pipelines)
    {
        _pipelines = pipelines;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<PipelineCompletedEvent> context)
    {
        var id = context.Message.Context
            .Get<string>("__pipeline_execution_id")
            .Then(Guid.Parse);

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

        var result = execution.Complete(context.Message.RaisedAt);
        if (result.IsError)
            await context.RejectAsync(result.ToFaultKind(), result.ToFault());
    }
}