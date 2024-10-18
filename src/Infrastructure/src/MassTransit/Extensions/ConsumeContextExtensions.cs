using Giantnodes.Infrastructure;

namespace MassTransit;

public static class ConsumeContextExtensions
{
    /// <summary>
    /// Extension method to reject a message by responding with a domain fault asynchronously.
    /// </summary>
    /// <param name="context">The ConsumeContext representing the message context.</param>
    /// <param name="kind">The kind of fault to be rejected.</param>
    /// <param name="property">An optional property that caused the error executing the operation.</param>
    /// <param name="errors">An optional collection of errors that occured executing the operation.</param>
    /// <returns>A task representing the asynchronous rejection operation.</returns>
    public static async Task RejectAsync(
        this ConsumeContext context,
        FaultKind kind,
        FaultProperty? property = null,
        IEnumerable<DomainFault.ErrorInfo>? errors = null)
    {
        var properties = Array.Empty<FaultProperty>();
        if (property != null)
            properties = [property];

        var fault = context.ToDomainFault(kind, properties, errors);
        if (!context.IsResponseAccepted<DomainFault>())
            throw new DomainException(fault);

        await context.RespondAsync(fault);
    }

    /// <summary>
    /// Extension method to reject a message by responding with a domain fault asynchronously.
    /// </summary>
    /// <param name="context">The ConsumeContext representing the message context.</param>
    /// <param name="kind">The kind of fault to be rejected.</param>
    /// <param name="errors">A collection of errors that occured executing the operation.</param>
    /// <returns>A task representing the asynchronous rejection operation.</returns>
    public static Task RejectAsync(
        this ConsumeContext context,
        FaultKind kind,
        IEnumerable<DomainFault.ErrorInfo> errors)
    {
        return RejectAsync(context, kind, null, errors);
    }

    /// <summary>
    /// Converts a MessageContext to a DomainFault based on the specified fault kind, with an optional property.
    /// </summary>
    /// <param name="context">The MessageContext representing the message context.</param>
    /// <param name="kind">The kind of fault to be converted.</param>
    /// <param name="properties">An optional collection of properties that caused the error executing the operation.</param>
    /// <param name="errors">An optional collection of errors that occured executing the operation.</param>
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