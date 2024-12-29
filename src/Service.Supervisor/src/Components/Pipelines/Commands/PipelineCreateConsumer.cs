using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines;

public sealed partial class PipelineCreateConsumer : IConsumer<PipelineCreate.Command>
{
    private readonly IPipelineService _service;

    public PipelineCreateConsumer(IPipelineService service)
    {
        _service = service;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<PipelineCreate.Command> context)
    {
        var name = PipelineName.Create(context.Message.Name);
        if (name.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, name.ToFault());
            return;
        }

        var slug = PipelineSlug.Create(name.Value);
        if (slug.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, slug.ToFault());
            return;
        }

        var pipeline = await _service.CreateAsync(
            name.Value,
            slug.Value,
            context.Message.Description,
            context.Message.Definition,
            context.CancellationToken);

        if (pipeline.IsError)
        {
            await context.RejectAsync(pipeline.ToFaultKind(), pipeline.ToFault());
            return;
        }

        await context.RespondAsync(new PipelineCreate.Result { PipelineId = pipeline.Value.Id });
    }
}