using Giantnodes.Infrastructure.Pipelines;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

internal sealed class ConverterPipeline : MassTransitPipeline, IConverterPipeline
{
    public ConverterPipeline(IPublishEndpoint endpoint, ILogger<ConverterPipeline> logger)
        : base(endpoint, logger)
    {
    }
}