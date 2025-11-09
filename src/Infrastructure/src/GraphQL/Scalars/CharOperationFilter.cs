using HotChocolate.Data.Filters;

namespace Giantnodes.Infrastructure.GraphQL;

internal sealed class CharOperationFilter : ComparableOperationFilterInputType<CharScalar>
{
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Name("CharOperationFilter");
        base.Configure(descriptor);
    }
}
