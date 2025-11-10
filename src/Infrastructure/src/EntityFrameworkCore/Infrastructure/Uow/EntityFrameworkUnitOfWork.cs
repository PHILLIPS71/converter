using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

/// <summary>
/// Entity Framework implementation of a unit of work that manages database transactions.
/// </summary>
/// <typeparam name="TDbContext">The type of Entity Framework DbContext to manage.</typeparam>
public sealed class EntityFrameworkUnitOfWork<TDbContext> : UnitOfWork
    where TDbContext : DbContext
{
    private readonly TDbContext _database;
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkUnitOfWork{TDbContext}"/> class.
    /// </summary>
    /// <param name="executor">The executor for running interceptors.</param>
    /// <param name="database">The Entity Framework DbContext to manage.</param>
    public EntityFrameworkUnitOfWork(IUnitOfWorkExecutor executor, TDbContext database)
        : base(executor)
    {
        _database = database;
    }

    protected override async Task OnBeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default)
    {
        if (options.Timeout.HasValue)
            _database.Database.SetCommandTimeout(options.Timeout.Value);

        var isTransactionRequired = options.Scope switch
        {
            TransactionScopeOption.Required => _database.Database.CurrentTransaction == null,
            TransactionScopeOption.RequiresNew => true,
            _ => false
        };

        if (isTransactionRequired)
            _transaction = await _database.Database.BeginTransactionAsync(cancellation);
    }

    /// <inheritdoc />
    protected override async Task OnCommitAsync(CancellationToken cancellation = default)
    {
        try
        {
            // collect domain events from tracked aggregate roots before saving
            foreach (var entry in _database.ChangeTracker.Entries<AggregateRoot>())
            {
                var events = entry.Entity.DomainEvents;
                if (events.Count <= 0)
                    continue;

                DomainEvents.AddRange(events);
                events.Clear();
            }

            await _database.SaveChangesAsync(cancellation);

            if (_transaction != null)
                await _database.Database.CommitTransactionAsync(cancellation);
        }
        catch
        {
            // if commit fails, ensure transaction is rolled back
            if (_transaction != null)
                await _database.Database.RollbackTransactionAsync(cancellation);

            throw;
        }
    }

    /// <inheritdoc />
    protected override async Task OnRollbackAsync(CancellationToken cancellation = default)
    {
        DomainEvents.Clear();

        if (_transaction != null)
            await _database.Database.RollbackTransactionAsync(cancellation);
    }

    /// <inheritdoc />
    protected override void Dispose(bool dispose)
    {
        if (dispose)
            _transaction?.Dispose();

        base.Dispose(dispose);
    }
}
