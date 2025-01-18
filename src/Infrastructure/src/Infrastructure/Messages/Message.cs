using MassTransit;

namespace Giantnodes.Infrastructure;

public abstract record Message
{
    public Guid MessageId { get; init; } = NewId.NextGuid();

    public Guid? CorrelationId { get; init; }
}