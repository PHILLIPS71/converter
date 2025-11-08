using ErrorOr;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

/// <summary>
/// MassTransit-based pipeline implementation that provides distributed, asynchronous pipeline execution through
/// message-based orchestration. This implementation is fire-and-forget, delegating execution to the pipeline state
/// machine for reliable, scalable processing.
/// </summary>
/// <remarks>
/// This implementation immediately returns success after publishing the execution command, making it suitable for
/// scenarios where immediate feedback is not required and execution can proceed asynchronously across distributed
/// workers.
/// </remarks>
internal sealed class MassTransitPipeline : IPipeline
{
    private readonly IPublishEndpoint _endpoint;
    private readonly ILogger<MassTransitPipeline> _logger;

    public MassTransitPipeline(IPublishEndpoint endpoint, ILogger<MassTransitPipeline> logger)
    {
        _endpoint = endpoint;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        CancellationToken cancellation = default)
    {
        var context = new PipelineContext();
        return await ExecuteAsync(definition, context, cancellation);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This implementation publishes a pipeline execution command and returns immediately. The actual execution is
    /// handled asynchronously by the pipeline state machine. Monitor pipeline lifecycle events
    /// (Started, Completed, Failed, Cancelled) to track progress.
    /// </remarks>
    public async Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default)
    {
        try
        {
            var @event = new PipelineExecute.Command
            {
                CorrelationId = Guid.NewGuid(),
                Pipeline = definition,
                Context = context
            };

            await _endpoint.Publish(@event, cancellation);

            // note: this returns success immediately after publishing, not after execution completion
            return Result.Success;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "failed to publish pipeline execution command for '{PipelineName}': {Error}", definition.Name, ex.Message);
            return Error.Unexpected(description: $"failed to publish pipeline execution command: {ex.Message}");
        }
    }
}
