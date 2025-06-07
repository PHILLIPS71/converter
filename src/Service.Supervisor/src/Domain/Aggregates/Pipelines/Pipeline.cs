using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public sealed class Pipeline : AggregateRoot<Guid>, ITimestampableEntity
{
    private Pipeline()
    {
        Executions = new List<PipelineExecution>();
    }

    private Pipeline(PipelineName name, PipelineSlug slug, string? description, string definition)
    {
        Id = NewId.NextSequentialGuid();
        Name = name;
        Slug = slug;
        Description = description;
        Definition = definition;
        Executions = new List<PipelineExecution>();
    }

    public static ErrorOr<Pipeline> Create(PipelineName name, PipelineSlug slug, string? description, string definition)
    {
        return new Pipeline(name, slug, description, definition);
    }

    public ErrorOr<Success> SetName(PipelineName name)
    {
        var slug = PipelineSlug.Create(name);
        if (slug.IsError)
            return slug.Errors;

        Name = name;
        Slug = slug.Value;

        return Result.Success;
    }

    public void SetDescription(string? description)
    {
        Description = description?.Trim();
    }

    public void SetDefinition(string definition)
    {
        Definition = definition;
    }

    public PipelineExecution CreateExecution(FileSystemFile file, string definition)
    {
        var execution = new PipelineExecution(this, definition, file);
        Executions.Add(execution);

        return execution;
    }

    public PipelineName Name { get; private set; }

    public PipelineSlug Slug { get; private set; }

    public string? Description { get; private set; }

    public string Definition { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public ICollection<PipelineExecution> Executions { get; private set; }
}