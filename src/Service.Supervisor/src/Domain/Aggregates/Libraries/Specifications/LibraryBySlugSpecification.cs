using System.Linq.Expressions;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Specifications;

public sealed class LibraryBySlugSpecification : Specification<Library>
{
    private readonly LibrarySlug _slug;

    public LibraryBySlugSpecification(LibrarySlug slug)
    {
        _slug = slug;
    }

    public override Expression<Func<Library, bool>> ToExpression()
    {
        return library => library.Slug == _slug;
    }
}
