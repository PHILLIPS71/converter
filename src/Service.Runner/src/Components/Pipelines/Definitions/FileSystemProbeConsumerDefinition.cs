using Giantnodes.Infrastructure.Pipelines.MassTransit;
using MassTransit;

namespace Giantnodes.Service.Runner.Components.Pipelines;

public sealed class PipelineSpecificationExecuteConsumerDefinition : ConsumerDefinition<PipelineSpecificationExecuteConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<PipelineSpecificationExecuteConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator
            .Options<JobOptions<PipelineSpecificationExecute.Job>>(options =>
                options
                    .SetRetry(r => r.Interval(3, TimeSpan.FromSeconds(30)))
                    .SetJobTimeout(TimeSpan.FromHours(48))
                    .SetConcurrentJobLimit(3)
            );
    }
}