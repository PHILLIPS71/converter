using ErrorOr;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public interface IPipelineService : IDomainService
{
    public Task<ErrorOr<Success>> IsPipelineUnique(
        Pipeline? pipeline,
        PipelineName name,
        PipelineSlug slug,
        CancellationToken cancellation = default);

    public Task<ErrorOr<Pipeline>> CreateAsync(
        PipelineName name,
        PipelineSlug slug,
        string? description,
        string definition,
        CancellationToken cancellation = default);

    public Task<ErrorOr<Pipeline>> UpdateAsync(
        Pipeline pipeline,
        CancellationToken cancellation = default);
}
