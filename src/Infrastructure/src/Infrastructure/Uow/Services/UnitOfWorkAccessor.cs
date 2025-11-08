namespace Giantnodes.Infrastructure;

/// <summary>
/// Implementation of unit of work accessor using AsyncLocal for thread-safe context isolation.
/// </summary>
internal sealed class UnitOfWorkAccessor : IUnitOfWorkAccessor
{
    private static readonly AsyncLocal<IUnitOfWorkContext?> Uow = new();

    /// <inheritdoc />
    public IUnitOfWorkContext? Context => Uow.Value;

    /// <inheritdoc />
    public void SetCurrent(IUnitOfWorkContext? context)
    {
        Uow.Value = context;
    }
}
