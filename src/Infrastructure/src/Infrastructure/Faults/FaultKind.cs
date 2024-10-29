namespace Giantnodes.Infrastructure;

public record FaultKind : Enumeration
{
    public static readonly FaultKind Unexpected =
        new(1, FaultType.Api, "unexpected", "an unexpected error occurred");

    public static readonly FaultKind Validation =
        new(2, FaultType.InvalidRequest, "validation", "one or more validation issues occurred");

    public static readonly FaultKind Domain =
        new(3, FaultType.InvalidRequest, "domain", "one or more domain rules were violated");

    public static readonly FaultKind NotFound =
        new(4, FaultType.InvalidRequest, "not_found", "the resource your are looking for cannot be found");

    public static readonly FaultKind OutOfRange =
        new(5, FaultType.InvalidRequest, "out_of_range", "the value provided is out of the allowed range");

    public static readonly FaultKind Constraint =
        new(6, FaultType.InvalidRequest, "constraint_violation", "the operation violates a unique constraint");

    public FaultKind(int id, FaultType type, string code, string message)
        : base(id, code)
    {
        Type = type;
        Code = code;
        Message = message;
    }

    /// <summary>
    /// The type of fault that can occur.
    /// </summary>
    public FaultType Type { get; private set; }

    /// <summary>
    /// The unique code of the kind of fault.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// The information about the kind of fault.
    /// </summary>
    public string Message { get; private set; }
}