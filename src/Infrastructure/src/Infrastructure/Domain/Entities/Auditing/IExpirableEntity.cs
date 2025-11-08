namespace Giantnodes.Infrastructure;

/// <summary>
/// An entity that tracks the <see cref="ExpiresAt" /> date and time of an entity.
/// </summary>
public interface IExpirableEntity
{
    /// <summary>
    /// The date and time when this entity expires.
    /// </summary>
    public DateTime ExpiresAt { get; }
}
