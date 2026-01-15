using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipelineBuilder
{
    public ErrorOr<PipelineDefinition> Build(string content, CancellationToken cancellation = default);
}
