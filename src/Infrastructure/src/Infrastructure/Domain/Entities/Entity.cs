namespace Giantnodes.Infrastructure;

/// <inheritdoc/>
public abstract class Entity : IEntity
{
    public abstract object[] GetKeys();
}

/// <inheritdoc cref="IEntity{TKey}" />
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    /// <inheritdoc/>
    public TKey Id { get; protected init; } = default!;

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}