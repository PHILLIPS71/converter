namespace Giantnodes.Infrastructure;

public sealed record DomainFault : IFault
{
    public readonly record struct ErrorInfo(string Code, string Description);

    /// <inheritdoc />
    public required Guid? RequestId { get; init; }

    /// <inheritdoc />
    public required FaultType Type { get; init; }

    /// <inheritdoc />
    public required string Code { get; init; }

    /// <inheritdoc />
    public required string Message { get; init; }

    /// <inheritdoc />
    public required DateTime TimeStamp { get; init; }

    /// <inheritdoc />
    public IEnumerable<FaultProperty> Properties { get; init; } = [];

    /// <summary>
    /// A collection of errors that occured executing the operation.
    /// </summary>
    public IEnumerable<ErrorInfo> Errors { get; init; } = [];
}
