namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipelineConfigurer
{
    IPipelineConfigurer AddPipeline<TPipeline, TResult>()
        where TPipeline : class, IPipeline<TResult>;
}