using ErrorOr;
using Giantnodes.Infrastructure;
using Slugify;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public sealed record PipelineSlug : ValueObject
{
    private PipelineSlug(string value) => Value = value;

    public string Value { get; }

    public static ErrorOr<PipelineSlug> Create(string value)
    {
        return new PipelineSlug(new SlugHelper().GenerateSlug(value));
    }

    public static ErrorOr<PipelineSlug> Create(PipelineName name)
    {
        return new PipelineSlug(new SlugHelper().GenerateSlug(name.Value));
    }
}