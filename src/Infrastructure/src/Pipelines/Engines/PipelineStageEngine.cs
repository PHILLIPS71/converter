using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines;

/// <inheritdoc />
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
        foreach (var step in stage.Steps)
        {
            cancellation.ThrowIfCancellationRequested();

            try
            {
                var executable = _factory.Create(step.Uses);
                if (executable.IsError)
                    return executable.Errors;

                var result = await executable.Value.ExecuteAsync(context, step, cancellation);
                if (result.IsError)
                    return result.Errors;

                var output = context.SetStepOutputs(step.Id, result.Value);
                if (output.IsError)
                    return output.Errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "unexpected error occurred in pipeline step. error: {Error}", ex.Message);
                return Error.Unexpected(description: $"unexpected error occurred in pipeline step. error: {ex.Message}");
            }
        }

        return Result.Success;
    }
}