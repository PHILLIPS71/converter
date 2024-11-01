using HotChocolate.Data.Filters;

namespace Giantnodes.Infrastructure.GraphQL.Scalars;

public sealed class CharOperationFilterInputType : ComparableOperationFilterInputType<CharType>
{
    protected override void Configure(IFilterInputTypeDescriptor descriptor)
    {
        descriptor.Name("CharOperationFilterInput");
        base.Configure(descriptor);
    }
}