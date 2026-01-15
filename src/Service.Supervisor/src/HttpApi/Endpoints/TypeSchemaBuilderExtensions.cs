using Giantnodes.Service.Supervisor.Domain.Values;
using HotChocolate.Execution.Configuration;

namespace Giantnodes.Service.Supervisor.HttpApi.Endpoints;

internal static class TypeSchemaBuilderExtensions
{
    public static IRequestExecutorBuilder AddDomainTypes(this IRequestExecutorBuilder builder)
    {
        return builder
            .BindRuntimeType<Name, StringType>()
            .AddTypeConverter<Name, string>(x => x.Value)
            .AddTypeConverter<string, Name>(x =>
            {
                var result = Name.Create(x);
                if (result.IsError)
                    throw new GraphQLException(ErrorBuilder.New().SetMessage(result.FirstError.Description).Build());

                return result.Value;
            })
            .BindRuntimeType<Slug, StringType>()
            .AddTypeConverter<Slug, string>(x => x.Value)
            .AddTypeConverter<string, Slug>(x =>
            {
                var result = Slug.Create(x);
                if (result.IsError)
                    throw new GraphQLException(ErrorBuilder.New().SetMessage(result.FirstError.Description).Build());

                return result.Value;
            });
    }
}
