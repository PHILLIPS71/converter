using ErrorOr;
using Giantnodes.Infrastructure;

namespace MassTransit;

public static class MessageContextExtensions
{
    /// <summary>
    /// Extension method to reject a message by responding with a domain fault asynchronously.
    /// </summary>
    /// <param name="context">The message context.</param>
    /// <param name="kind">The kind of fault to be rejected.</param>
    /// <param name="property">An optional property that caused the error executing the operation.</param>
    /// <param name="errors">An optional collection of errors that occurred executing the operation.</param>
    /// <returns>A task representing the asynchronous rejection operation.</returns>
    public static async Task RejectAsync<TContext>(
        this TContext context,
        FaultKind kind,
        FaultProperty? property = null,
        IEnumerable<DomainFault.ErrorInfo>? errors = null)
        where TContext : class, MessageContext
    {
        var properties = Array.Empty<FaultProperty>();
        if (property != null)
            properties = [property];

        var fault = context.ToDomainFault(kind, properties, errors);

        switch (context)
        {
            case ConsumeContext consume:
                if (!consume.IsResponseAccepted<DomainFault>())
                    throw new DomainException(fault);

                await consume.RespondAsync(fault);
                break;

            default:
                throw new DomainException(fault);
        }
    }

    /// <summary>
    /// Extension method to reject a message by responding with a domain fault asynchronously.
    /// </summary>
    /// <param name="context">The message context.</param>
    /// <param name="kind">The kind of fault to be rejected.</param>
    /// <param name="errors">A collection of errors that occurred executing the operation.</param>
    /// <returns>A task representing the asynchronous rejection operation.</returns>
    public static Task RejectAsync<TContext>(
        this TContext context,
        FaultKind kind,
        IEnumerable<DomainFault.ErrorInfo> errors)
        where TContext : class, MessageContext
    {
        return RejectAsync(context, kind, null, errors);
    }

    /// <summary>
    /// Extension method to reject a message by responding with a domain fault asynchronously using an Error object.
    /// </summary>
    /// <param name="context">The message context.</param>
    /// <param name="error">The error object to be converted to a domain fault.</param>
    /// <returns>A task representing the asynchronous rejection operation.</returns>
    public static Task RejectAsync<TContext>(
        this TContext context,
        Error error)
        where TContext : class, MessageContext
    {
        return RejectAsync(context, error.ToFaultKind(), [error.ToFault()]);
    }

    /// <summary>
    /// Converts a MessageContext to a DomainFault based on the specified fault kind, with optional properties.
    /// </summary>
    /// <param name="context">The MessageContext representing the message context.</param>
    /// <param name="kind">The kind of fault to be converted.</param>
    /// <param name="properties">An optional collection of properties that caused the error executing the operation.</param>
    /// <param name="errors">An optional collection of errors that occurred executing the operation.</param>
    /// <returns>A DomainFault representing the converted fault.</returns>
    private static DomainFault ToDomainFault(
        this MessageContext context,
        FaultKind kind,
        IEnumerable<FaultProperty>? properties = null,
        IEnumerable<DomainFault.ErrorInfo>? errors = null)
    {
        return new DomainFault
        {
            Type = kind.Type,
            RequestId = context.RequestId,
            TimeStamp = InVar.Timestamp,
            Code = kind.Code,
            Message = kind.Message,
            Properties = properties ?? [],
            Errors = errors ?? []
        };
    }
}
