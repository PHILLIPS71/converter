namespace Giantnodes.Infrastructure;

/// <inheritdoc cref="IAggregateRoot" />
public abstract class AggregateRoot : Entity, IAggregateRoot, IHasConcurrencyToken
{
    public readonly ICollection<DomainEvent> DomainEvents = [];

    public byte[]? ConcurrencyToken { get; private set; }
}

/// <inheritdoc cref="IAggregateRoot{TKey}" />
public class AggregateRoot<TKey> : AggregateRoot, IAggregateRoot<TKey>
{
    public TKey Id { get; protected init; } = default!;

    public override object[] GetKeys()
    {
        return [Id];
    }
}
