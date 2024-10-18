using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

public class UpperCaseConverter : ValueConverter<string, string>
{
    public UpperCaseConverter()
        : base(
            v => v.ToUpperInvariant(),
            v => v.ToUpperInvariant())
    {
    }
}