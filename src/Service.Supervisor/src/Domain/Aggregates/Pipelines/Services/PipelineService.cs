using ErrorOr;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

internal sealed class PipelineService : IPipelineService
{
    private readonly IPipelineRepository _pipelines;

    public PipelineService(IPipelineRepository pipelines)
    {
        _pipelines = pipelines;
    }

    public async Task<ErrorOr<Success>> IsPipelineUnique(
        Pipeline? pipeline,
        PipelineName name,
        PipelineSlug slug,
        CancellationToken cancellation = default)
    {
        var duplicate = await _pipelines
            .FirstOrDefaultAsync(x => (pipeline == null || x.Id != pipeline.Id) && (x.Name == name || x.Slug == slug), cancellation);

        if (duplicate != null)
        {
            if (duplicate.Name == name)
                return Error.Conflict(description: $"a pipeline with name '{name}' already exists");

            return Error.Conflict(description: $"a pipeline with slug '{slug}' already exists");
        }

        return Result.Success;
    }

    public async Task<ErrorOr<Pipeline>> CreateAsync(
        PipelineName name,
        PipelineSlug slug,
        string? description,
        string definition,
        CancellationToken cancellation = default)
    {
        var isUnique = await IsPipelineUnique(null, name, slug, cancellation);
        if (isUnique.IsError)
            return isUnique.Errors;

        var pipeline = Pipeline.Create(name, slug, description, definition);
        if (pipeline.IsError)
            return pipeline.Errors;

        return _pipelines.Create(pipeline.Value);
    }

    public async Task<ErrorOr<Pipeline>> UpdateAsync(Pipeline pipeline, CancellationToken cancellation = default)
    {
        var isUnique = await IsPipelineUnique(pipeline, pipeline.Name, pipeline.Slug, cancellation);
        if (isUnique.IsError)
            return isUnique.Errors;

        return _pipelines.Update(pipeline);
    }
}