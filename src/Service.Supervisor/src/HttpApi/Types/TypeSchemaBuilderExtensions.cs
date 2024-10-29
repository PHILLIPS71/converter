using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using HotChocolate.Execution.Configuration;

namespace Giantnodes.Service.Supervisor.HttpApi.Types;

internal static class TypeSchemaBuilderExtensions
{
    public static IRequestExecutorBuilder AddDomainTypes(this IRequestExecutorBuilder builder)
    {
        return builder
            .BindRuntimeType<LibraryName, StringType>()
            .AddTypeConverter<LibraryName, string>(x => x.Value)
            .AddTypeConverter<string, LibraryName>(x => LibraryName.Create(x).Value)

            .BindRuntimeType<LibrarySlug, StringType>()
            .AddTypeConverter<LibrarySlug, string>(x => x.Value)
            .AddTypeConverter<string, LibrarySlug>(x => LibrarySlug.Create(x));
    }
}