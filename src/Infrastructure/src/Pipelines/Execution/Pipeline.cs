using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines;

/// <inheritdoc />
public abstract class Pipeline<TResult> : IPipeline<TResult>
{
    private readonly IPipelineEngine _engine;
    private readonly ILogger<Pipeline<TResult>> _logger;

    protected Pipeline(
        IPipelineEngine engine,
        ILogger<Pipeline<TResult>> logger)
    {
        _engine = engine;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ErrorOr<TResult>> ExecuteAsync(
        PipelineDefinition definition,
        CancellationToken cancellation = default)
    {
        var context = new PipelineContext();
        return await ExecuteAsync(definition, context, cancellation);
    }

    /// <inheritdoc />
    public async Task<ErrorOr<TResult>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default)
    {
        try
        {
            var result = await _engine.ExecuteAsync(definition, context, cancellation);
            if (result.IsError)
                return result.Errors;

            return CreateResult(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "unexpected error occurred in pipeline. Error: {Error}", ex.Message);
            return Error.Unexpected(description: $"unexpected error occurred in pipeline. Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates the final result from the pipeline context after all stages and steps have executed.
    /// </summary>
    /// <param name="context">The context containing data from the pipeline execution.</param>
    /// <returns>The typed result of the pipeline.</returns>
    protected abstract TResult CreateResult(PipelineContext context);
}