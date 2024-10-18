using System.Transactions;

namespace Giantnodes.Infrastructure;

public sealed class UnitOfWorkOptions
{
    public TransactionScopeOption Scope { get; init; } = TransactionScopeOption.Required;

    public TimeSpan? Timeout { get; init; }
}