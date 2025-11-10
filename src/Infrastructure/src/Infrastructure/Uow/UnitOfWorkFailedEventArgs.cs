namespace Giantnodes.Infrastructure;

public sealed class UnitOfWorkFailedEventArgs : EventArgs
{
    /// <summary>
    /// The exception that caused the unit of work to fail.
    /// </summary>
    public Exception Exception { get; init; }

    public UnitOfWorkFailedEventArgs(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        Exception = exception;
    }
}
