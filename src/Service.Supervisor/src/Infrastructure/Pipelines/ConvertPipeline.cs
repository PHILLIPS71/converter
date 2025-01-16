using Giantnodes.Infrastructure.Pipelines;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

public sealed class ConvertPipeline : MassTransitPipeline, IConvertPipeline
{
    public ConvertPipeline(IPublishEndpoint endpoint, ILogger<ConvertPipeline> logger)
        : base(endpoint, logger)
    {
    }
}