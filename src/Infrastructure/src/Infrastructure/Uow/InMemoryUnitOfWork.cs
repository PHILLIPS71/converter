namespace Giantnodes.Infrastructure;

/// <summary>
/// In-memory implementation of a unit of work that provides basic transaction semantics without persistent storage.
/// </summary>
/// <remarks>
/// This implementation is useful for testing scenarios or applications that don't require database persistence.
/// </remarks>
public sealed class InMemoryUnitOfWork : UnitOfWork
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryUnitOfWork"/> class.
    /// </summary>
    /// <param name="executor">The executor for running interceptors.</param>
    public InMemoryUnitOfWork(IUnitOfWorkExecutor executor) : base(executor)
    {
    }

    /// <inheritdoc />
    protected override Task OnBeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnCommitAsync(CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task OnRollbackAsync(CancellationToken cancellation = default)
    {
        DomainEvents.Clear();
        return Task.CompletedTask;
    }
}
