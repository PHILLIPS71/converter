using System.Linq.Expressions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Specifications;

public sealed class LibraryByNameSpecification : Specification<Library>
{
    private readonly Name _name;

    public LibraryByNameSpecification(Name name)
    {
        _name = name;
    }

    public override Expression<Func<Library, bool>> ToExpression()
    {
        return library => library.Name == _name;
    }
}
