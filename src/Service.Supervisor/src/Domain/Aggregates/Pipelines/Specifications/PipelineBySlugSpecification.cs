using System.Linq.Expressions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines.Specifications;

public sealed class PipelineBySlugSpecification : Specification<Pipeline>
{
    private readonly Slug _slug;

    public PipelineBySlugSpecification(Slug slug)
    {
        _slug = slug;
    }

    public override Expression<Func<Pipeline, bool>> ToExpression()
        => pipeline => pipeline.Slug == _slug;
}
