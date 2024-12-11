namespace Giantnodes.Infrastructure.Pipelines;

internal interface IPipelineConfigurer
{
    void AddPipeline<TPipeline, TInput, TResult>()
        where TPipeline : class, IPipeline<TInput, TResult>;
}