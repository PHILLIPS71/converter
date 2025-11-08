using System.Transactions;

namespace Giantnodes.Infrastructure;

/// <summary>
/// Configuration options for Unit of Work transaction behavior.
/// </summary>
public sealed class UnitOfWorkOptions
{
    /// <summary>
    /// Transaction scope behavior. Defaults to Required.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><see cref="TransactionScopeOption.Required"/> - Uses existing transaction if available, otherwise creates new</item>
    /// <item><see cref="TransactionScopeOption.RequiresNew"/> - Always creates a new transaction</item>
    /// <item><see cref="TransactionScopeOption.Suppress"/> - Executes without a transaction</item>
    /// </list>
    /// </remarks>
    public TransactionScopeOption Scope { get; init; } = TransactionScopeOption.Required;

    /// <summary>
    /// Transaction timeout duration. Null uses the system default timeout.
    /// </summary>
    /// <remarks>
    /// When specified, this timeout applies to the entire Unit of Work operation. If the operation exceeds this
    /// timeout, the transaction will be aborted.
    /// </remarks>
    public TimeSpan? Timeout { get; init; }
}
