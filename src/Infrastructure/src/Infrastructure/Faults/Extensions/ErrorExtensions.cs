using Giantnodes.Infrastructure;

namespace ErrorOr;

public static class ErrorExtensions
{
    public static IEnumerable<DomainFault.ErrorInfo> ToFault<TValue>(this ErrorOr<TValue> error)
    {
        return error.Errors.Select(x => new DomainFault.ErrorInfo(x.Code, x.Description));
    }

    public static DomainFault.ErrorInfo ToFault(this Error error)
    {
        return new DomainFault.ErrorInfo(error.Code, error.Description);
    }
}