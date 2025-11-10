namespace Giantnodes.Infrastructure;

public abstract record Message
{
    public Guid MessageId { get; init; } = Ulid.NewUlid().ToGuid();

    public Guid? CorrelationId { get; init; }
}
