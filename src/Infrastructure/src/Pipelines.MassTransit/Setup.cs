using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public static class Setup
{
    /// <summary>
    /// Configures MassTransit to use the pipeline saga state machine.
    /// </summary>
    /// <param name="configurator">The <see cref="IBusRegistrationConfigurator" /> for the bus being configured.</param>
    public static void AddGiantnodesPipelines(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddSagaStateMachine<PipelineStateMachine, PipelineSagaState>();
    }
}