using HotChocolate.Data.Filters;

namespace Giantnodes.Infrastructure.GraphQL;

public sealed class IdOperationFilter : StringOperationFilterInputType
{
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Name("IdOperationFilter");

        descriptor.Operation(DefaultFilterOperations.Equals).Type<StringType>();
        descriptor.Operation(DefaultFilterOperations.In).Type<ListType<StringType>>();
    }
}
