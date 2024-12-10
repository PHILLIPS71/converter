using ErrorOr;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

internal interface IPipelineSpecificationFactory
{
    ErrorOr<IPipelineSpecification<TContext>> Create<TContext>(string name, IDictionary<string, object>? inputs = null) 
        where TContext : PipelineContext;
}