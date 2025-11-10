using System.Linq.Expressions;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Specifications;

public sealed class LibraryByNameSpecification : Specification<Library>
{
    private readonly LibraryName _name;

    public LibraryByNameSpecification(LibraryName name)
    {
        _name = name;
    }

    public override Expression<Func<Library, bool>> ToExpression()
    {
        return library => library.Name == _name;
    }
}
