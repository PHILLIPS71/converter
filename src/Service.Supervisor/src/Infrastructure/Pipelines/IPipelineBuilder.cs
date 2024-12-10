using ErrorOr;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

public interface IPipelineBuilder
{
    ErrorOr<PipelineDefinition> Build(string content, CancellationToken cancellation = default);
}