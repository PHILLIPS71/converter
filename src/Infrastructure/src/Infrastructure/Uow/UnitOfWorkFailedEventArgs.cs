namespace Giantnodes.Infrastructure;

public sealed class UnitOfWorkFailedEventArgs : EventArgs
{
    /// <summary>
    /// The exception that caused the unit of work to fail.
    /// </summary>
    public Exception Exception { get; private set; }

    public UnitOfWorkFailedEventArgs(Exception exception)
    {
        Exception = exception;
    }
}