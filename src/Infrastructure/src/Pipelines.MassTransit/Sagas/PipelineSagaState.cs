using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed class PipelineSagaState : SagaStateMachineInstance, IHasConcurrencyToken
{
    public Guid CorrelationId { get; set; }

    public string? CurrentState { get; set; }

    public PipelineDefinition? Pipeline { get; set; }

    public PipelineContext? Context { get; set; }

    public List<PipelineStageSagaState> Stages { get; set; } = [];

    public byte[]? ConcurrencyToken { get; set; }
}

public sealed class PipelineStageSagaState
{
    public PipelineStageDefinition? Stage { get; set; }

    public Guid? JobId { get; set; }

    public int Dependencies { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool IsCompleted() => JobId.HasValue && CompletedAt != null;
}
