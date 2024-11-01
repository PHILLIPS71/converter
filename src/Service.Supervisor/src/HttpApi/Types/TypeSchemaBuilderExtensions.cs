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
            .AddTypeConverter<string, LibraryName>(x =>
            {
                var result = LibraryName.Create(x);
                if (result.IsError)
                    throw new GraphQLException(ErrorBuilder.New().SetMessage(result.FirstError.Description).Build());

                return result.Value;
            })
            .BindRuntimeType<LibrarySlug, StringType>()
            .AddTypeConverter<LibrarySlug, string>(x => x.Value)
            .AddTypeConverter<string, LibrarySlug>(LibrarySlug.Create);
    }
}