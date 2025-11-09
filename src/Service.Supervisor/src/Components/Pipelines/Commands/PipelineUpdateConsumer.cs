using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines.Specifications;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines;

public sealed partial class PipelineUpdateConsumer : IConsumer<PipelineUpdate.Command>
{
    private readonly IPipelineRepository _repository;

    public PipelineUpdateConsumer(IPipelineRepository repository)
    {
        _repository = repository;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<PipelineUpdate.Command> context)
    {
        var pipeline = await _repository.SingleOrDefaultAsync(new IdSpecification<Pipeline, Id>(context.Message.Id), context.CancellationToken);
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

        var slug = PipelineSlug.Create(name.Value);
        if (slug.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, slug.ToFault());
            return;
        }

        var duplicate = await _repository.FirstOrDefaultAsync(
            new PipelineUniquenessSpecification(name.Value, slug.Value, pipeline.Id),
            context.CancellationToken);

        if (duplicate != null)
        {
            var error = duplicate.Name == name.Value
                ? Error.Conflict(description: $"a pipeline with name '{name.Value}' already exists")
                : Error.Conflict(description: $"a pipeline with slug '{slug.Value}' already exists");

            await context.RejectAsync(error.ToFaultKind(), error.ToFault());
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

        _repository.Update(pipeline);

        await context.RespondAsync(new PipelineUpdate.Result { PipelineId = pipeline.Id });
    }
}
