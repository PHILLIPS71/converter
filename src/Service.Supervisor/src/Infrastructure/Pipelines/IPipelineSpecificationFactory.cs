using ErrorOr;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

public interface IPipelineSpecificationFactory
{
    ErrorOr<IPipelineSpecification> Create(string name, IDictionary<string, object>? inputs = null);
}