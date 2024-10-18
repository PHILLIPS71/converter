namespace Giantnodes.Infrastructure;

public abstract class UnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWorkExecutor _executor;

    private bool _started;
    private bool _committed;
    private Exception? _exception;

    protected List<Event> DomainEvents { get; }

    public Guid CorrelationId { get; }

    public Guid? UserId { get; private set; }

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
        DomainEvents = new List<Event>();
    }

    public async Task<IUnitOfWork> BeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (_started)
            throw new InvalidOperationException("The Uow has already started.");

        Options = options;

        try
        {
            await OnBeginAsync(options, cancellation);
            await _executor.OnAfterBeginAsync(this, cancellation);
            _started = true;
            return this;
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    public async Task CommitAsync(CancellationToken cancellation = default)
    {
        if (_committed)
            throw new InvalidOperationException("The Uow has already been committed.");

        try
        {
            await OnCommitAsync(cancellation);
            await _executor.OnAfterCommitAsync(this, cancellation);
            _committed = true;
            Completed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    public void SetUserId(Guid id)
    {
        UserId = id;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose" />
    protected virtual void Dispose(bool dispose)
    {
        if (!dispose || !_started || IsDisposed)
            return;

        IsDisposed = true;

        if (_exception != null)
            Failed?.Invoke(this, new UnitOfWorkFailedEventArgs(_exception));

        Disposed?.Invoke(this, EventArgs.Empty);
    }

    protected abstract Task OnBeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default);

    protected abstract Task OnCommitAsync(CancellationToken cancellation = default);
}