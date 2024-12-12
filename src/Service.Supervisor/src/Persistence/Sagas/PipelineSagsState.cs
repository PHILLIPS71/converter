using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.Pipelines;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Persistence.Sagas;

public sealed class PipelineSagsState : SagaStateMachineInstance, IHasConcurrencyToken
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; }

    public PipelineDefinition Definition { get; set; }

    public int Specification { get; set; }

    public Guid JobId { get; set; }

    public byte[]? ConcurrencyToken { get; }
}