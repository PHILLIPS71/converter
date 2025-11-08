namespace Giantnodes.Infrastructure;

/// <summary>
/// A interface defines a contract for entities that can be soft-deleted.
/// </summary>
public interface ISoftDeletableEntity
{
    /// <summary>
    /// A boolean property that indicates whether the entity has been deleted.
    /// </summary>
    public bool IsDeleted { get; }

    /// <summary>
    /// A nullable DateTime property that represents the date and time when the entity was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; }
}
