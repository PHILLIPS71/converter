namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipelineConfigurer
{
    IPipelineConfigurer AddPipeline<TPipeline, TResult>()
        where TPipeline : class, IPipeline<TResult>;

    IPipelineConfigurer AddPipeline<TInterface, TPipeline, TResult>()
        where TInterface : class, IPipeline<TResult>
        where TPipeline : class, TInterface;

    IPipelineConfigurer AddSpecification<TSpecification>()
        where TSpecification : IPipelineSpecification;
}