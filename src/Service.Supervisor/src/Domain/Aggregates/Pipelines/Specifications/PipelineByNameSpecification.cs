using System.Linq.Expressions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines.Specifications;

public sealed class PipelineByNameSpecification : Specification<Pipeline>
{
    private readonly Name _name;

    public PipelineByNameSpecification(Name name)
    {
        _name = name;
    }

    public override Expression<Func<Pipeline, bool>> ToExpression()
        => pipeline => pipeline.Name == _name;
}
