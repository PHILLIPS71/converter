using Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;
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
            .BindRuntimeType<PipelineName, StringType>()
            .AddTypeConverter<PipelineName, string>(x => x.Value)
            .AddTypeConverter<string, PipelineName>(x =>
            {
                var result = PipelineName.Create(x);
                if (result.IsError)
                    throw new GraphQLException(ErrorBuilder.New().SetMessage(result.FirstError.Description).Build());

                return result.Value;
            })
            .BindRuntimeType<LibrarySlug, StringType>()
            .AddTypeConverter<LibrarySlug, string>(x => x.Value)
            .AddTypeConverter<string, LibrarySlug>(x =>
            {
                var result = LibrarySlug.Create(x);
                if (result.IsError)
                    throw new GraphQLException(ErrorBuilder.New().SetMessage(result.FirstError.Description).Build());

                return result.Value;
            })
            .BindRuntimeType<PipelineSlug, StringType>()
            .AddTypeConverter<PipelineSlug, string>(x => x.Value)
            .AddTypeConverter<string, PipelineSlug>(x =>
            {
                var result = PipelineSlug.Create(x);
                if (result.IsError)
                    throw new GraphQLException(ErrorBuilder.New().SetMessage(result.FirstError.Description).Build());

                return result.Value;
            });
    }
}
