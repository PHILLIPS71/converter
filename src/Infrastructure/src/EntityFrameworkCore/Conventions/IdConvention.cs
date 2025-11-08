using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

internal sealed class IdConvention : IPropertyAddedConvention
{
    public void ProcessPropertyAdded(
        IConventionPropertyBuilder propertyBuilder,
        IConventionContext<IConventionPropertyBuilder> context)
    {
        var type = propertyBuilder.Metadata.ClrType;

        var converter = type switch
        {
            _ when type == typeof(Id) => new IdValueConverter(),
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Id<>) =>
                (ValueConverter?)Activator.CreateInstance(typeof(IdValueConverter<>).MakeGenericType(type.GetGenericArguments().First())),
            _ => null
        };

        if (converter != null)
            propertyBuilder.HasConversion(converter);
    }
}
