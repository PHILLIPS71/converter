namespace Giantnodes.Infrastructure;

/// <inheritdoc cref="IAggregateRoot" />
public abstract class AggregateRoot : Entity, IAggregateRoot, IHasConcurrencyToken
{
    public readonly ICollection<DomainEvent> DomainEvents = [];

    public byte[]? ConcurrencyToken { get; private set; }
}

/// <inheritdoc cref="IAggregateRoot{TKey}" />
public class AggregateRoot<TId> : AggregateRoot, IAggregateRoot<TId>
    where TId : IId
{
    public TId Id { get; protected init; } = default!;

    public override object[] GetKeys() => [Id];
}
