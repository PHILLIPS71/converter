using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Infrastructure.Pipelines.MassTransit;
using MassTransit;

namespace Giantnodes.Service.Runner.Components.Pipelines;

public sealed class PipelineSpecificationExecuteConsumer : IJobConsumer<PipelineSpecificationExecute.Job>
{
    private readonly IPipelineSpecificationFactory _factory;

    public PipelineSpecificationExecuteConsumer(IPipelineSpecificationFactory factory)
    {
        _factory = factory;
    }

    public async Task Run(JobContext<PipelineSpecificationExecute.Job> context)
    {
        var specification = _factory.Create(context.Job.Specification.Uses);
        if (specification.IsError)
            throw new InvalidOperationException(specification.FirstError.Description);

        var result = await specification.Value.ExecuteAsync(context.Job.Specification, new PipelineContext(context.Job.State), context.CancellationToken);
        if (result.IsError)
            throw new InvalidOperationException(result.FirstError.Description);
    }
}