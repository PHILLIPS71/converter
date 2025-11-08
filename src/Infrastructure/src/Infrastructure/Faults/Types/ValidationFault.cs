namespace Giantnodes.Infrastructure;

public sealed record ValidationFault : IFault
{
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
}
