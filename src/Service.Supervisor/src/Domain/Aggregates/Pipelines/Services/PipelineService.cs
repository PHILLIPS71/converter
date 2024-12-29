using ErrorOr;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines.Services;

internal sealed class PipelineService : IPipelineService
{
    private readonly IPipelineRepository _pipelines;

    public PipelineService(IPipelineRepository pipelines)
    {
        _pipelines = pipelines;
    }

    public async Task<ErrorOr<Pipeline>> CreateAsync(
        PipelineName name,
        PipelineSlug slug,
        string? description,
        string definition,
        CancellationToken cancellation = default)
    {
        var isNameUsed = await _pipelines.ExistsAsync(x => x.Name == name, cancellation);
        if (isNameUsed)
            return Error.Conflict(description: $"a pipeline with name '{name}' already exists");

        var isSlugUsed = await _pipelines.ExistsAsync(x => x.Slug == slug, cancellation);
        if (isSlugUsed)
            return Error.Conflict(description: $"a pipeline with slug '{slug}' already exists");

        var pipeline = Pipeline.Create(name, slug, description, definition);
        if (pipeline.IsError)
            return pipeline.Errors;

        return _pipelines.Create(pipeline.Value);
    }
}