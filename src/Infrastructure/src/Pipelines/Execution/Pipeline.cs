using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines;

/// <inheritdoc />
internal sealed class Pipeline : IPipeline
{
    private readonly IPipelineEngine _engine;
    private readonly ILogger<Pipeline> _logger;

    public Pipeline(IPipelineEngine engine, ILogger<Pipeline> logger)
    {
        _engine = engine;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        CancellationToken cancellation = default)
    {
        var context = new PipelineContext(Guid.NewGuid());
        return await ExecuteAsync(definition, context, cancellation);
    }

    /// <inheritdoc />
    public async Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default)
    {
        try
        {
            var result = await _engine.ExecuteAsync(definition, context, cancellation);
            if (result.IsError)
                return result.Errors;

            return Result.Success;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "unexpected error occurred in pipeline. Error: {Error}", ex.Message);
            return Error.Unexpected(description: $"unexpected error occurred in pipeline. Error: {ex.Message}");
        }
    }
}
