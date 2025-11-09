using System.Linq.Expressions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Directories;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Specifications;

public sealed class DirectoryByPathSpecification : Specification<FileSystemDirectory>
{
    private readonly string _path;

    public DirectoryByPathSpecification(string path)
    {
        _path = path;
    }

    public override Expression<Func<FileSystemDirectory, bool>> ToExpression()
    {
        return directory => directory.PathInfo.FullName == _path;
    }
}
