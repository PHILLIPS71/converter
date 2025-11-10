namespace Giantnodes.Infrastructure;

/// <summary>
/// An entity that tracks its version to prevent concurrency conflicts via optimistic concurrency.
/// </summary>
public interface IHasConcurrencyToken
{
    /// <summary>
    /// A concurrency token used as a version of the entity.
    /// </summary>
    public byte[]? ConcurrencyToken { get; }
}
