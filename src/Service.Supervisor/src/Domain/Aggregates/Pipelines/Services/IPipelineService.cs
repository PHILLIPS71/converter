using ErrorOr;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public interface IPipelineService
{
    public Task<ErrorOr<Pipeline>> CreateAsync(
        PipelineName name,
        PipelineSlug slug,
        string? description,
        string definition,
        CancellationToken cancellation = default);
}