namespace Giantnodes.Infrastructure;

public interface IFault
{
    /// <summary>
    /// Identifies the request that caused the fault.
    /// </summary>
    public Guid? RequestId { get; }

    /// <summary>
    /// The type of fault that occurred.
    /// </summary>
    public FaultType Type { get; }

    /// <summary>
    /// The code of the fault that occurred.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// The information about the fault that occured.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// The time when fault occured.
    /// </summary>
    public DateTime TimeStamp { get; }

    /// <summary>
    /// A collection of properties that caused the fault to occur.
    /// </summary>
    public IEnumerable<FaultProperty> Properties { get; init; }
}