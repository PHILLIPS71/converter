using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

public sealed class EntityFrameworkUnitOfWork<TDbContext> : UnitOfWork
    where TDbContext : DbContext
{
    private readonly TDbContext _database;
    private IDbContextTransaction? _transaction;

    public EntityFrameworkUnitOfWork(IUnitOfWorkExecutor executor, TDbContext database)
        : base(executor)
    {
        _database = database;
    }

    protected override async Task OnBeginAsync(UnitOfWorkOptions options, CancellationToken cancellation = default)
    {
        if (options.Timeout.HasValue)
            _database.Database.SetCommandTimeout(options.Timeout.Value);

        var isTransactionRequired =
            options.Scope == TransactionScopeOption.Required && _database.Database.CurrentTransaction == null
            || options.Scope == TransactionScopeOption.RequiresNew;

        if (isTransactionRequired)
            _transaction = await _database.Database.BeginTransactionAsync(cancellation);
    }

    protected override async Task OnCommitAsync(CancellationToken cancellation = default)
    {
        var events = _database
            .ChangeTracker
            .Entries<AggregateRoot>()
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        DomainEvents.AddRange(events);

        await _database.SaveChangesAsync(cancellation);

        if (_transaction != null)
            await _database.Database.CommitTransactionAsync(cancellation);
    }

    protected override async Task OnRollbackAsync(CancellationToken cancellation = default)
    {
        DomainEvents.Clear();

        if (_transaction != null)
            await _database.Database.RollbackTransactionAsync(cancellation);
    }
}
