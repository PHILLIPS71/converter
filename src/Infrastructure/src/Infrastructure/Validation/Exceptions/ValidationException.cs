namespace Giantnodes.Infrastructure;

public class ValidationException : Exception
{
    public ValidationFault Fault { get; }

    public ValidationException(ValidationFault fault)
        : base(fault.Message)
    {
        Fault = fault;
    }
}
