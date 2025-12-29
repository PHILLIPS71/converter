using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed class PipelineSagaState : SagaStateMachineInstance, IHasConcurrencyToken
{
    public required Guid CorrelationId { get; set; }

    public string? CurrentState { get; set; }

    public required PipelineDefinition Pipeline { get; set; }

    public required PipelineContext Context { get; set; }

    public Dictionary<string, StageExecutionState> Stages { get; set; } = [];

    public byte[]? ConcurrencyToken { get; set; }
}

public sealed class StageExecutionState
{
    public PipelineStageDefinition? Stage { get; set; }

    public Guid? JobId { get; set; }

    public int Dependencies { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool IsCompleted()
        => JobId.HasValue && CompletedAt != null;
}
