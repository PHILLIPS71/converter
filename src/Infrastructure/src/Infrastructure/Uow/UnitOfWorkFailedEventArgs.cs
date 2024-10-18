namespace Giantnodes.Infrastructure;

public sealed class UnitOfWorkFailedEventArgs : EventArgs
{
    public Exception Exception { get; private set; }

    public UnitOfWorkFailedEventArgs(Exception exception)
    {
        Exception = exception;
    }
}