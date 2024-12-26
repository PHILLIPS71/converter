using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

internal sealed class PipelineSpecificationFactory : IPipelineSpecificationFactory
{
    private readonly IEnumerable<IPipelineSpecification> _specifications;

    public PipelineSpecificationFactory(IEnumerable<IPipelineSpecification> specifications)
    {
        _specifications = specifications;
    }

    public ErrorOr<IPipelineSpecification> Create(string name)
    {
        var specification = _specifications.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (specification == null)
            return Error.NotFound(description: $"specification '{name}' cannot be found");

        return ErrorOrFactory.From(specification);
    }
}