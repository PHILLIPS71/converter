namespace Giantnodes.Infrastructure;

/// <inheritdoc/>
public abstract class Entity : IEntity
{
    public abstract object[] GetKeys();
}

/// <inheritdoc cref="IEntity{TKey}" />
public abstract class Entity<TId> : Entity, IEntity<TId>
    where TId : IId
{
    /// <inheritdoc/>
    public TId Id { get; protected init; } = default!;

    public override object[] GetKeys() => [Id];
}
