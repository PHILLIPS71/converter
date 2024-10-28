using System.Transactions;

namespace Giantnodes.Infrastructure;

public sealed class UnitOfWorkOptions
{
    /// <summary>
    /// Transaction scope option. Defaults to Required (uses existing transaction if available; otherwise, creates new).
    /// </summary>
    public TransactionScopeOption Scope { get; init; } = TransactionScopeOption.Required;

    /// <summary>
    /// Transaction timeout. Null uses the system default timeout.
    /// </summary>
    public TimeSpan? Timeout { get; init; }
}