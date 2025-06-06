﻿using ErrorOr;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public abstract class MassTransitPipeline : IPipeline<Success>
{
    private readonly IPublishEndpoint _endpoint;
    private readonly ILogger<MassTransitPipeline> _logger;

    protected MassTransitPipeline(IPublishEndpoint endpoint, ILogger<MassTransitPipeline> logger)
    {
        _endpoint = endpoint;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        CancellationToken cancellation = default)
    {
        var context = new PipelineContext();
        return await ExecuteAsync(definition, context, cancellation);
    }

    public async Task<ErrorOr<Success>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default)
    {
        try
        {
            var @event = new PipelineExecute.Command
            {
                Definition = definition,
                State = context.State,
            };

            await _endpoint.Publish(@event, cancellation);
            return Result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "an unexpected error occurred executing pipeline. error: {Error}", ex.Message);
            return Error.Unexpected(description: $"an unexpected error occurred executing pipeline. error: {ex.Message}");
        }
    }
}