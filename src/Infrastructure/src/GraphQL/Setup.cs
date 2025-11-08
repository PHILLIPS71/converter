using HotChocolate.Data.Filters;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Descriptors;
using Microsoft.Extensions.DependencyInjection;

namespace Giantnodes.Infrastructure.GraphQL;

public static class Setup
{
    public static IRequestExecutorBuilder AddPlatformConfiguration(this IRequestExecutorBuilder builder)
    {
        builder
            .AddGraphQLTypes()
            .AddConvention<INamingConventions, PlatformNamingConvention>()
            .AddConvention<IFilterConvention, PlatformFilterConvention>();

        return builder;
    }
}
