using System.Text.RegularExpressions;
using ErrorOr;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Values;

/// <summary>
/// Represents a validated name value object that contains only alphanumeric characters and spaces.
/// </summary>
public sealed partial record Name : ValueObject
{
    [GeneratedRegex(@"^[a-zA-Z0-9\s]+$")]
    private static partial Regex ValidNamePattern();

    private Name(string value) => Value = value;

    /// <summary>
    /// Gets the string value of the name.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new Name instance after validating the input value.
    /// </summary>
    /// <param name="value">The string value to validate and convert to a Name.</param>
    /// <returns>
    /// An ErrorOr containing either a valid Name or validation errors.
    /// </returns>
    public static ErrorOr<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(description: "a name cannot be empty");

        if (!ValidNamePattern().IsMatch(value))
            return Error.Validation(description: "a name can only contain letters and numbers");

        return new Name(value);
    }
}
