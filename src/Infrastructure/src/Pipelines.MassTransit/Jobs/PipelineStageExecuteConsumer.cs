using ErrorOr;
using MassTransit;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<PipelineStageExecuteConsumer> _logger;

    public PipelineStageExecuteConsumer(IPipelineOperationFactory factory, ILogger<PipelineStageExecuteConsumer> logger)
    {
        _factory = factory;
        _logger = logger;
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
            {
                _logger.LogError("failed to create operation '{Uses}' for step '{Id}': {Description}", definition.Uses, definition.Id, step.FirstError.Description);
                await context.RejectAsync(step.ToFaultKind(), step.ToFault());
                return;
            }

            // execute the step and handle any errors
            var result = await step.Value.ExecuteAsync(definition, context.Job.Context, context.CancellationToken);
            if (result.IsError)
            {
                _logger.LogError("step '{Id}' failed: {Error}", definition.Id, step.FirstError.Description);
                await context.RejectAsync(result.ToFaultKind(), result.ToFault());
                return;
            }
        }
    }
}
