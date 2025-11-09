using System.Linq.Expressions;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines.Specifications;

public sealed class PipelineBySlugSpecification : Specification<Pipeline>
{
    private readonly PipelineSlug _slug;

    public PipelineBySlugSpecification(PipelineSlug slug)
    {
        _slug = slug;
    }

    public override Expression<Func<Pipeline, bool>> ToExpression()
        => pipeline => pipeline.Slug == _slug;
}
