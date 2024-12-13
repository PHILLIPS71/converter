namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipelineConfigurer
{
    void AddPipeline<TPipeline, TInput, TResult>()
        where TPipeline : class, IPipeline<TInput, TResult>;
}