namespace Giantnodes.Infrastructure;

public class DomainException : Exception
{
    public DomainFault Fault { get; set; }

    public DomainException(DomainFault fault)
        : base(fault.Message)
    {
        Fault = fault;
    }
}