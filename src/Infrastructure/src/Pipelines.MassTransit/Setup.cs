using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

/// <summary>
/// Configuration extensions for pipeline orchestration and execution using MassTransit.
/// </summary>
public static class Setup
{
    /// <summary>
    /// Configures the pipeline orchestrator that manages saga workflow and coordinates pipeline execution.
    /// </summary>
    /// <param name="configurator">The <see cref="IBusRegistrationConfigurator"/> for the bus being configured.</param>
    /// <remarks>
    /// This method registers the <see cref="PipelineStateMachine"/> and <see cref="PipelineSagaState"/> which act as
    /// the orchestrator for all pipeline operations.
    /// </remarks>
    public static void AddPipelineOrchestrator(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddSagaStateMachine<PipelineStateMachine, PipelineSagaState>();
    }

    /// <summary>
    /// Configures a pipeline worker that executes individual pipeline stages.
    /// </summary>
    /// <param name="configurator">The <see cref="IBusRegistrationConfigurator"/> for the bus being configured.</param>
    /// <remarks>
    /// This method registers the <see cref="PipelineStageExecuteConsumer"/> which acts as a distributed worker
    /// responsible for executing pipeline stages.
    /// </remarks>
    public static void AddPipelineWorker(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<PipelineStageExecuteConsumer>();
    }
}