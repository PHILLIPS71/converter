using System.Linq.Expressions;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines.Specifications;

public sealed class PipelineUniquenessSpecification : Specification<Pipeline>
{
    private readonly ISpecification<Pipeline> _specification;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineUniquenessSpecification"/> class that filters pipelines
    /// by name or slug, optionally excluding a specific pipeline by ID.
    /// </summary>
    /// <param name="name">The pipeline name to check for uniqueness.</param>
    /// <param name="slug">The pipeline slug to check for uniqueness.</param>
    /// <param name="id">Optional pipeline ID to exclude from the uniqueness check, typically used when updating an existing pipeline.</param>
    public PipelineUniquenessSpecification(Name name, Slug slug, Id? id = null)
    {
        var uniqueness = new OrSpecification<Pipeline>(
            new PipelineByNameSpecification(name),
            new PipelineBySlugSpecification(slug));

        _specification = id.HasValue
            ? new AndSpecification<Pipeline>(uniqueness, new NoneSpecification<Pipeline>(new IdSpecification<Pipeline, Id>(id.Value)))
            : uniqueness;
    }

    public override Expression<Func<Pipeline, bool>> ToExpression()
        => _specification.ToExpression();
}
