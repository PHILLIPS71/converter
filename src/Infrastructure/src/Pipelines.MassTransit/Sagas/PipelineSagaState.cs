using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed class PipelineSagaState : SagaStateMachineInstance, IHasConcurrencyToken
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; }

    public PipelineDefinition Definition { get; set; }

    public PipelineContext Context { get; set; }

    public int Specification { get; set; }

    public Guid? JobId { get; set; }

    public byte[]? ConcurrencyToken { get; }
}