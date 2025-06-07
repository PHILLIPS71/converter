using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Engine responsible for the sequential execution of steps within a pipeline stage. Each stage contains multiple steps
/// that must be executed in order, with the output of each step being captured and made available to subsequent
/// pipeline stages.
/// </summary>
internal sealed class PipelineStageEngine : IPipelineStageEngine
{
    private readonly IPipelineOperationFactory _factory;
    private readonly ILogger<PipelineStageEngine> _logger;

    public PipelineStageEngine(
        IPipelineOperationFactory factory,
        ILogger<PipelineStageEngine> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ErrorOr<Success>> ExecuteAsync(
        PipelineContext context,
        PipelineStageDefinition stage,
        CancellationToken cancellation = default)
    {
        // execute each step in the stage sequentially
        foreach (var step in stage.Steps)
        {
            cancellation.ThrowIfCancellationRequested();

            try
            {
                // resolve the operation implementation for this step
                var executable = _factory.Create(step.Uses);
                if (executable.IsError)
                    return executable.Errors;

                // execute the step and capture its outputs
                var result = await executable.Value.ExecuteAsync(step, context, cancellation);
                if (result.IsError)
                    return result.Errors;

                // store step outputs for use by subsequent steps/stages
                var output = context.SetStepOutputs(step.Id, result.Value);
                if (output.IsError)
                    return output.Errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "unexpected error occurred in pipeline step '{StepId}': {Error}", step.Id, ex.Message);
                return Error.Unexpected(description: $"unexpected error occurred in pipeline step '{step.Id}': {ex.Message}");
            }
        }

        return Result.Success;
    }
}