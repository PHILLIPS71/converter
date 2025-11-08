using HotChocolate.Data.Filters;

namespace Giantnodes.Infrastructure.GraphQL;

internal sealed class PlatformFilterConvention : FilterConvention
{
    protected override void Configure(IFilterConventionDescriptor descriptor)
    {
        descriptor.AddDefaults();

        // todo: https://github.com/ChilliCream/graphql-platform/pull/8601
        descriptor.BindRuntimeType<Id, IdOperationFilter>();
        descriptor.BindRuntimeType(typeof(Id<>), typeof(IdOperationFilter));

        // descriptor.BindRuntimeType<char, CharOperationFilter>();
        // descriptor.BindRuntimeType<char?, CharOperationFilter>();
    }
}
