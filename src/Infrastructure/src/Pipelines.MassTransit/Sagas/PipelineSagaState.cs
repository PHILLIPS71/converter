using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines.MassTransit;

public sealed class PipelineSagaState : SagaStateMachineInstance, IHasConcurrencyToken
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; }

    public PipelineDefinition Pipeline { get; set; }

    public PipelineContext Context { get; set; }

    public Dictionary<string, int> Pending { get; set; } = [];

    public Dictionary<string, Guid> Executing { get; set; } = [];

    public List<string> Completed { get; set; } = [];

    public byte[]? ConcurrencyToken { get; set; }
}