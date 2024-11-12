using Giantnodes.Infrastructure;

namespace ErrorOr;

public static class ErrorExtensions
{
    public static FaultKind ToFaultKind<TValue>(this ErrorOr<TValue> error)
    {
        return error.FirstError.ToFaultKind();
    }

    public static FaultKind ToFaultKind(this Error error)
    {
        return error.Type switch
        {
            ErrorType.Failure => FaultKind.Unexpected,
            ErrorType.Unexpected => FaultKind.Unexpected,
            ErrorType.Validation => FaultKind.Validation,
            ErrorType.Conflict => FaultKind.Constraint,
            ErrorType.NotFound => FaultKind.NotFound,
            ErrorType.Unauthorized => FaultKind.Unexpected,
            ErrorType.Forbidden => FaultKind.Unexpected,
            _ => FaultKind.Unexpected,
        };
    }

    public static IEnumerable<DomainFault.ErrorInfo> ToFault<TValue>(this ErrorOr<TValue> error)
    {
        return error.Errors.Select(x => new DomainFault.ErrorInfo(x.Code, x.Description));
    }

    public static DomainFault.ErrorInfo ToFault(this Error error)
    {
        return new DomainFault.ErrorInfo(error.Code, error.Description);
    }
}