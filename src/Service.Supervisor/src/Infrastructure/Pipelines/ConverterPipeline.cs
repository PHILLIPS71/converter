using Giantnodes.Infrastructure.Pipelines.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

public sealed class ConverterPipeline : MassTransitPipeline
{
    public ConverterPipeline(IPublishEndpoint endpoint, ILogger<ConverterPipeline> logger)
        : base(endpoint, logger)
    {
    }
}