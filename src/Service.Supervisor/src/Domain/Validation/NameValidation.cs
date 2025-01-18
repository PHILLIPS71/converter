using System.Text.RegularExpressions;
using ErrorOr;

namespace Giantnodes.Service.Supervisor.Domain.Validation;

public static partial class NameValidation
{
    [GeneratedRegex(@"^[a-zA-Z0-9\s]+$")]
    private static partial Regex ValidNamePattern();

    public static bool IsValidName(string value) => ValidNamePattern().IsMatch(value);

    public static ErrorOr<string> ValidateName(string value, string entity)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation(description: $"a {entity.ToLower()} name cannot be empty");

        if (!IsValidName(value))
            return Error.Validation(description: $"a {entity.ToLower()} name can only contain letters and numbers");

        return value;
    }
}