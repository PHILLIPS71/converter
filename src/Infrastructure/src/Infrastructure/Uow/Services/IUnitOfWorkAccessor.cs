namespace Giantnodes.Infrastructure;

/// <summary>
/// Provides access to the current unit of work context within the current execution scope.
/// </summary>
public interface IUnitOfWorkAccessor
{
    /// <summary>
    /// Gets the current unit of work context, if available.
    /// </summary>
    /// <value>
    /// The current unit of work context, or <see langword="null"/> if no unit of work is active.
    /// </value>
    public IUnitOfWorkContext? Context { get; }

    /// <summary>
    /// Sets the current unit of work context for the current execution scope.
    /// </summary>
    /// <param name="context">The unit of work context to set as current.</param>
    /// <remarks>
    /// This method is typically called by the unit of work infrastructure and should not be used directly.
    /// </remarks>
    public void SetCurrent(IUnitOfWorkContext? context);
}
