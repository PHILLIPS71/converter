using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Validation;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public sealed record LibraryName : ValueObject
{
    private LibraryName(string value) => Value = value;

    public string Value { get; }

    public static ErrorOr<LibraryName> Create(string value)
    {
        var result = NameValidation.ValidateName(value, nameof(Library));
        if (result.IsError)
            return result.Errors;

        return new LibraryName(result.Value);
    }
}