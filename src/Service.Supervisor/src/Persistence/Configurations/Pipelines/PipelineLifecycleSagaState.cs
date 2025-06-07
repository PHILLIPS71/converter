using Giantnodes.Infrastructure;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Persistence.Configurations;

public sealed class PipelineLifecycleSagaState : SagaStateMachineInstance, IHasConcurrencyToken
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; }

    public byte[]? ConcurrencyToken { get; set; }
}