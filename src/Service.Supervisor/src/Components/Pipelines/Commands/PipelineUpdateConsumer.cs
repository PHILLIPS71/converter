using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines;

public sealed partial class PipelineUpdateConsumer : IConsumer<PipelineUpdate.Command>
{
    private readonly IPipelineRepository _repository;
    private readonly IPipelineService _service;

    public PipelineUpdateConsumer(IPipelineRepository repository, IPipelineService service)
    {
        _repository = repository;
        _service = service;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<PipelineUpdate.Command> context)
    {
        var pipeline = await _repository.SingleOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);
        if (pipeline == null)
        {
            await context.RejectAsync(FaultKind.NotFound, FaultProperty.Create(context.Message.Id));
            return;
        }

        var name = PipelineName.Create(context.Message.Name);
        if (name.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, name.ToFault());
            return;
        }

        var update = pipeline.SetName(name.Value);
        if (update.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, update.ToFault());
            return;
        }

        pipeline.SetDescription(context.Message.Description);
        pipeline.SetDefinition(context.Message.Definition);

        var result = await _service.UpdateAsync(pipeline, context.CancellationToken);
        if (result.IsError)
        {
            await context.RejectAsync(result.ToFaultKind(), result.ToFault());
            return;
        }

        await context.RespondAsync(new PipelineUpdate.Result { PipelineId = pipeline.Id });
    }
}