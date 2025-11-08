namespace Giantnodes.Infrastructure;

/// <summary>
/// Represents the various states a unit of work can be in during its lifecycle.
/// </summary>
public enum UnitOfWorkState
{
    /// <summary>
    /// Unit of work has been created but not yet started.
    /// </summary>
    Created,

    /// <summary>
    /// Unit of work has been started and is active.
    /// </summary>
    Started,

    /// <summary>
    /// Unit of work has been successfully committed.
    /// </summary>
    Committed,

    /// <summary>
    /// Unit of work has been rolled back.
    /// </summary>
    RolledBack,

    /// <summary>
    /// Unit of work has been disposed.
    /// </summary>
    Disposed
}
