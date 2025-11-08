using Giantnodes.Service.Runner.Contracts.Probing.Jobs;
using MassTransit;

namespace Giantnodes.Service.Runner.Components.Probing;

public sealed class FileSystemProbeConsumerDefinition : ConsumerDefinition<FileSystemProbeConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<FileSystemProbeConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        consumerConfigurator
            .Options<JobOptions<FileSystemProbe.Job>>(options =>
                options
                    .SetRetry(r => r.Interval(3, TimeSpan.FromSeconds(30)))
                    .SetJobTimeout(TimeSpan.FromHours(3))
                    .SetConcurrentJobLimit(3)
            );
    }
}
