using Giantnodes.Infrastructure.GraphQL.Scalars;
using HotChocolate.Data.Filters;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Descriptors;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure.GraphQL;

public static class Setup
{
    public static IRequestExecutorBuilder AddGiantnodesConfiguration(this IRequestExecutorBuilder builder)
    {
        builder
            .AddType<CharType>()
            .AddConvention<IFilterConvention, CharFilterConvention>()
            .AddConvention<INamingConventions, GiantnodesNamingConvention>()
            .AddGraphQLTypes();

        return builder;
    }
}