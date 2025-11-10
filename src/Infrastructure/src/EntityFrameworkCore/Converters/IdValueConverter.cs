using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

internal sealed class IdValueConverter : ValueConverter<Id, string>
{
    public IdValueConverter()
        : base(
            id => id.ToString(),
            value => new Id(value))
    {
    }
}

internal class IdValueConverter<T> : ValueConverter<Id<T>, string>
{
    public IdValueConverter()
        : base(
            id => id.ToString(),
            value => new Id<T>(value))
    {
    }
}
