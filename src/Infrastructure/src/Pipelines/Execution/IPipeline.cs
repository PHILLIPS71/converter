using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipeline<TResult>
{
    Task<ErrorOr<TResult>> ExecuteAsync(
        PipelineDefinition definition,
        CancellationToken cancellation = default);

    Task<ErrorOr<TResult>> ExecuteAsync(
        PipelineDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default);
}