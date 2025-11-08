namespace Giantnodes.Infrastructure;

public enum FaultType
{
    /// <summary>
    /// arise when internal errors occur and cover any other type of problem.
    /// </summary>
    Api,

    /// <summary>
    /// arise when an idempotency key is reused on a request that does not match the first requests
    /// endpoint or parameters.
    /// </summary>
    Idempotency,

    /// <summary>
    /// arise when a request has invalid parameters or in an invalid state.
    /// </summary>
    InvalidRequest,

    /// <summary>
    /// arise when too many requests are sent to the api too quickly.
    /// </summary>
    RateLimit
}
