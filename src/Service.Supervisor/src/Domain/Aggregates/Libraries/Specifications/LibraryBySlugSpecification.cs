using System.Linq.Expressions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Specifications;

public sealed class LibraryBySlugSpecification : Specification<Library>
{
    private readonly Slug _slug;

    public LibraryBySlugSpecification(Slug slug)
    {
        _slug = slug;
    }

    public override Expression<Func<Library, bool>> ToExpression()
    {
        return library => library.Slug == _slug;
    }
}
