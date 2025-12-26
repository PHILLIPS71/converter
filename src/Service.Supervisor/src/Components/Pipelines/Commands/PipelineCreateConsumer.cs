using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Contracts.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines.Specifications;
using Giantnodes.Service.Supervisor.Domain.Values;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Components.Pipelines;

public sealed partial class PipelineCreateConsumer : IConsumer<PipelineCreate.Command>
{
    private readonly IPipelineRepository _repository;

    public PipelineCreateConsumer(IPipelineRepository repository)
    {
        _repository = repository;
    }

    [UnitOfWork]
    public async Task Consume(ConsumeContext<PipelineCreate.Command> context)
    {
        var name = Name.Create(context.Message.Name);
        if (name.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, name.ToFault());
            return;
        }

        var slug = Slug.Create(name.Value);
        if (slug.IsError)
        {
            await context.RejectAsync(FaultKind.Validation, slug.ToFault());
            return;
        }

        var duplicate = await _repository.FirstOrDefaultAsync(
            new PipelineUniquenessSpecification(name.Value, slug.Value),
            context.CancellationToken);

        if (duplicate != null)
        {
            var error = duplicate.Name == name.Value
                ? Error.Conflict(description: $"a pipeline with name '{name.Value}' already exists")
                : Error.Conflict(description: $"a pipeline with slug '{slug.Value}' already exists");

            await context.RejectAsync(error.ToFaultKind(), error.ToFault());
            return;
        }

        var pipeline = Pipeline.Create(name.Value, slug.Value, context.Message.Description, context.Message.Definition);
        if (pipeline.IsError)
        {
            await context.RejectAsync(pipeline.ToFaultKind(), pipeline.ToFault());
            return;
        }

        _repository.Create(pipeline.Value);
        await context.RespondAsync(new PipelineCreate.Result { PipelineId = pipeline.Value.Id });
    }
}
