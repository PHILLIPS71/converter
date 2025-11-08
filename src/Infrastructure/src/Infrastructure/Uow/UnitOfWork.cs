namespace Giantnodes.Infrastructure;

public abstract class UnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWorkExecutor _executor;

    private Exception? _exception;

    protected List<Event> DomainEvents { get; }

    public Guid CorrelationId { get; }

    public Guid? UserId { get; private set; }

    public bool IsStarted { get; private set; }

    public bool IsCommitted { get; private set; }

    public bool IsDisposed { get; private set; }

    public event EventHandler? Completed;

    public event EventHandler? Failed;

    public event EventHandler? Disposed;

    public UnitOfWorkOptions? Options { get; private set; }

    public IReadOnlyCollection<object> Events => DomainEvents.ToList().AsReadOnly();

    protected UnitOfWork(IUnitOfWorkExecutor executor)
    {
        _executor = executor;

        CorrelationId = Guid.NewGuid();
        DomainEvents = [];
    }

    public void SetUserId(Guid id)
    {
        UserId = id;
    }

    /// <inheritdoc cref="IUnitOfWork.BeginAsync"/>
    public async Task<IUnitOfWork> BeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (IsStarted)
            throw new InvalidOperationException("The Uow has already started.");

        Options = options;

        try
        {
            await OnBeginAsync(options, cancellation);
            await _executor.OnAfterBeginAsync(this, cancellation);
            IsStarted = true;
            return this;
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    /// <inheritdoc cref="IUnitOfWork.CommitAsync"/>
    public async Task CommitAsync(CancellationToken cancellation = default)
    {
        if (IsCommitted)
            throw new InvalidOperationException("The Uow has already been committed.");

        try
        {
            await OnCommitAsync(cancellation);
            await _executor.OnAfterCommitAsync(this, cancellation);
            IsCommitted = true;
            Completed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    /// <inheritdoc cref="IUnitOfWork.RollbackAsync"/>
    public async Task RollbackAsync(CancellationToken cancellation = default)
    {
        if (!IsStarted)
            throw new InvalidOperationException("Cannot rollback a unit of work that hasn't started.");

        if (IsCommitted)
            throw new InvalidOperationException("Cannot rollback a unit of work that has already been committed.");

        try
        {
            await OnRollbackAsync(cancellation);
            IsCommitted = true;
            Completed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    /// <inheritdoc cref="DisposeAsync" />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    /// <see cref="DisposeAsync" />
    protected virtual async ValueTask DisposeAsync(bool dispose)
    {
        if (!dispose || !IsStarted || IsDisposed)
            return;

        try
        {
            await RollbackAsync();
        }
        catch (Exception ex)
        {
            _exception = ex;
        }

        if (_exception != null)
            Failed?.Invoke(this, new UnitOfWorkFailedEventArgs(_exception));

        IsDisposed = true;
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Executes when the unit of work begins. Implementations should initialize their transaction
    /// and perform any required setup.
    /// </summary>
    /// <param name="options">Configuration options for the unit of work</param>
    /// <param name="cancellation">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    protected abstract Task OnBeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default);

    /// <summary>
    /// Executes when the unit of work is committed. Implementations should save changes
    /// and commit their transaction.
    /// </summary>
    /// <param name="cancellation">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    protected abstract Task OnCommitAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Executes when the unit of work is rolled back. Implementations should revert changes
    /// and rollback their transaction.
    /// </summary>
    /// <param name="cancellation">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    protected abstract Task OnRollbackAsync(CancellationToken cancellation = default);
}
