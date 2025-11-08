namespace Giantnodes.Infrastructure;

/// <summary>
/// Base implementation of a unit of work that coordinates multiple operations as a single transaction.
/// </summary>
public abstract class UnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWorkExecutor _executor;

    private Exception? _exception;

    /// <summary>
    /// Domain events collected during this unit of work that will be published after commit.
    /// </summary>
    protected List<Event> DomainEvents { get; }

    /// <inheritdoc />
    public Guid CorrelationId { get; }

    /// <inheritdoc />
    public Id? UserId { get; private set; }

    /// <inheritdoc />
    public Id? TenantId { get; private set; }

    /// <inheritdoc />
    public UnitOfWorkState State { get; private set; } = UnitOfWorkState.Created;

    /// <inheritdoc />
    public event EventHandler? Completed;

    /// <inheritdoc />
    public event EventHandler? Failed;

    /// <inheritdoc />
    public event EventHandler? Disposed;

    /// <inheritdoc />
    public UnitOfWorkOptions? Options { get; private set; }

    /// <summary>
    /// Domain events that will be published after this unit of work commits.
    /// </summary>
    public IReadOnlyCollection<Event> Events => DomainEvents.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="executor">The executor responsible for running interceptors.</param>
    protected UnitOfWork(IUnitOfWorkExecutor executor)
    {
        _executor = executor;

        CorrelationId = Ulid.NewUlid().ToGuid();
        DomainEvents = [];
    }

    /// <summary>
    /// Sets the user ID for this unit of work context.
    /// </summary>
    /// <param name="id">The user ID to associate with this unit of work.</param>
    public void SetUserId(Id id)
    {
        UserId = id;
    }

    /// <summary>
    /// Sets the tenant ID for this unit of work context.
    /// </summary>
    /// <param name="id">The tenant ID to associate with this unit of work.</param>
    public void SetTenantId(Id id)
    {
        TenantId = id;
    }

    /// <inheritdoc cref="IUnitOfWork.BeginAsync"/>
    public async Task<IUnitOfWork> BeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.Timeout.HasValue && options.Timeout <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(options), "timeout must be greater than zero.");

        if (State == UnitOfWorkState.Started)
            throw new InvalidOperationException("the Uow has already started.");

        Options = options;

        try
        {
            await OnBeginAsync(options, cancellation);
            await _executor.OnAfterBeginAsync(this, cancellation);
            State = UnitOfWorkState.Started;
            return this;
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellation = default)
    {
        if (State != UnitOfWorkState.Started)
            throw new InvalidOperationException("cannot commit a Uow that has not started.");

        if (State == UnitOfWorkState.Committed)
            throw new InvalidOperationException("cannot commit a Uow that has already been committed.");

        try
        {
            await OnCommitAsync(cancellation);
            await _executor.OnAfterCommitAsync(this, cancellation);
            State = UnitOfWorkState.Committed;
            Completed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellation = default)
    {
        if (State != UnitOfWorkState.Started)
            throw new InvalidOperationException("cannot rollback a unit of work that hasn't started.");

        if (State == UnitOfWorkState.Committed)
            throw new InvalidOperationException("cannot rollback a unit of work that has already been committed.");

        try
        {
            await OnRollbackAsync(cancellation);
            State = UnitOfWorkState.RolledBack;
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    protected virtual void Dispose(bool dispose)
    {
        if (!dispose || State == UnitOfWorkState.Disposed)
            return;

        State = UnitOfWorkState.Disposed;

        if (_exception != null)
            Failed?.Invoke(this, new UnitOfWorkFailedEventArgs(_exception));

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
