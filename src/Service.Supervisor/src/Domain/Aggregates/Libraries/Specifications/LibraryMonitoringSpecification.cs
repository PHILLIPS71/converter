using System.Linq.Expressions;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries.Specifications;

public sealed class LibraryMonitoringSpecification : Specification<Library>
{
    private readonly bool _isMonitoring;

    public LibraryMonitoringSpecification(bool isMonitoring)
    {
        _isMonitoring = isMonitoring;
    }

    public override Expression<Func<Library, bool>> ToExpression()
        => library => library.IsMonitoring == _isMonitoring;
}
