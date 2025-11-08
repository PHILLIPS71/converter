using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

/// <summary>
/// Job consumer responsible for executing individual pipeline stages in a distributed environment. Processes stage
/// execution commands by running all steps within the stage sequentially.
/// </summary>
/// <remarks>
/// This consumer integrates with MassTransit's job service to provide reliable, scalable execution of pipeline stages
/// across distributed worker instances. Each stage runs as a separate job, allowing for parallel execution of
/// independent stages.
/// </remarks>
internal sealed class PipelineStageExecuteConsumer : IJobConsumer<PipelineStageExecute.Command>
{
    private readonly IPipelineOperationFactory _factory;

    public PipelineStageExecuteConsumer(IPipelineOperationFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Executes all steps within the specified pipeline stage sequentially.
    /// </summary>
    /// <param name="context">The job context containing the stage to execute and execution metadata.</param>
    /// <returns>A task representing the asynchronous execution operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when step creation fails or step execution returns errors.
    /// This approach preserves error information while integrating with MassTransit's job system.
    /// </exception>
    public async Task Run(JobContext<PipelineStageExecute.Command> context)
    {
        var stage = context.Job.Stage;

        // execute each step in the stage sequentially
        foreach (var definition in stage.Steps)
        {
            // resolve the operation implementation for this step
            var step = _factory.Create(definition.Uses);
            if (step.IsError)
                throw new InvalidOperationException($"failed to create operation '{definition.Uses}' for step '{definition.Id}': {step.FirstError.Description}");

            // execute the step and handle any errors
            var result = await step.Value.ExecuteAsync(definition, context.Job.Context, context.CancellationToken);
            if (result.IsError)
                throw new InvalidOperationException($"step '{definition.Id}' failed: {result.FirstError.Description}");
        }
    }
}
