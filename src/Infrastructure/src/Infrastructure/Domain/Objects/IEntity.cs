namespace Giantnodes.Infrastructure;

/// <summary>
/// A entity where it's primary key is make up by a combination of keys.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Combination of keys that make up the unique identifier for this entity.
    /// </summary>
    object[] GetKeys();
}

/// <summary>
/// A entity with a single primary key property.
/// </summary>
/// <typeparam name="TKey">Type of primary key property for the entity.</typeparam>
public interface IEntity<out TKey> : IEntity
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    TKey Id { get; }
}
