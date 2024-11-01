using System.Text.RegularExpressions;
using ErrorOr;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public sealed partial record LibraryName : ValueObject
{
    [GeneratedRegex(@"^[a-zA-Z0-9\s]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidNamePattern();

    private LibraryName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ErrorOr<LibraryName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(description: "a library name cannot be empty");

        if (!ValidNamePattern().IsMatch(value))
            return Error.Validation(description: "a library name can only contain letters, numbers, and spaces");

        return new LibraryName(value);
    }
}