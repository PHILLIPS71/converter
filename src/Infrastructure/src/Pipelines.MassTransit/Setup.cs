using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

/// <summary>
/// Configuration extensions for pipeline orchestration and execution using MassTransit.
/// </summary>
public static class Setup
{
    /// <summary>
    /// Configures the pipeline to use MassTransit to enable distributed pipeline execution.
    /// </summary>
    /// <param name="configurer">The <see cref="IPipelineConfigurer"/> being configured.</param>
    /// <returns>The same <see cref="IPipelineConfigurer"/> instance for method chaining.</returns>
    public static IPipelineConfigurer UseMassTransit(this IPipelineConfigurer configurer)
    {
        configurer.Services.TryAddScoped<IPipeline, MassTransitPipeline>();
        configurer.Services.TryAddScoped<MassTransitPipeline>();

        return configurer;
    }

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