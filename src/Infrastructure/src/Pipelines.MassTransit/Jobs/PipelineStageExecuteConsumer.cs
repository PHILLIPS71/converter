using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

internal sealed class PipelineStageExecuteConsumer : IJobConsumer<PipelineStageExecute.Command>
{
    private readonly IPipelineOperationFactory _factory;

    public PipelineStageExecuteConsumer(IPipelineOperationFactory factory)
    {
        _factory = factory;
    }

    public async Task Run(JobContext<PipelineStageExecute.Command> context)
    {
        foreach (var definition in context.Job.Stage.Steps)
        {
            var step = _factory.Create(definition.Uses);
            if (step.IsError)
                throw new InvalidOperationException(step.FirstError.Description);

            var result = await step.Value.ExecuteAsync(definition, context.Job.Context, context.CancellationToken);
            if (result.IsError)
                throw new InvalidOperationException(result.FirstError.Description);
        }
    }
}