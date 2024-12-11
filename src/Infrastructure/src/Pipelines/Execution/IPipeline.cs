using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipeline<in TInput, TResult>
{
    Task<ErrorOr<TResult>> ExecuteAsync(
        PipelineDefinition definition,
        TInput input,
        CancellationToken cancellation = default);
}