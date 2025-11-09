using System.Linq.Expressions;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines.Specifications;

public sealed class PipelineByNameSpecification : Specification<Pipeline>
{
    private readonly PipelineName _name;

    public PipelineByNameSpecification(PipelineName name)
    {
        _name = name;
    }

    public override Expression<Func<Pipeline, bool>> ToExpression()
        => pipeline => pipeline.Name == _name;
}
