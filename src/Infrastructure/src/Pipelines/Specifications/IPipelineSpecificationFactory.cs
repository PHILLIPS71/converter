using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipelineSpecificationFactory
{
    ErrorOr<IPipelineSpecification> Create(string name);
}