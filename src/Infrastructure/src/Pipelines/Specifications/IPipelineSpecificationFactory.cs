using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

internal interface IPipelineSpecificationFactory
{
    ErrorOr<IPipelineSpecification<TContext>> Create<TContext>(string name, IDictionary<string, object>? inputs = null) 
        where TContext : PipelineContext;
}