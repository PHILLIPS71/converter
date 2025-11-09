using System.Linq.Expressions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Specifications;

public sealed class FileByPathSpecification : Specification<FileSystemFile>
{
    private readonly ICollection<string> _paths;

    public FileByPathSpecification(ICollection<string> paths)
    {
        _paths = paths;
    }

    public FileByPathSpecification(string path)
    {
        _paths = [path];
    }

    public override Expression<Func<FileSystemFile, bool>> ToExpression()
        => entry => _paths.Contains(entry.PathInfo.FullName);
}
