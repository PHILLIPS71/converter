using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipelineBuilder
{
    ErrorOr<PipelineDefinition> Build(string content, CancellationToken cancellation = default);
}