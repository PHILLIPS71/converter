using System.Linq.Expressions;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Specifications;

public sealed class LibraryByPathSpecification : Specification<Library>
{
    private readonly string _path;

    public LibraryByPathSpecification(string path)
    {
        _path = path;
    }

    public override Expression<Func<Library, bool>> ToExpression()
    {
        return library => library.Directory.PathInfo.FullName == _path;
    }
}
