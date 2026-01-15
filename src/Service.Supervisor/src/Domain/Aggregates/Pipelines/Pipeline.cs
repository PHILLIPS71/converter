using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Values;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public sealed class Pipeline : AggregateRoot<Id>, ITimestampableEntity
{
    private Pipeline()
    {
        Executions = [];
    }

    private Pipeline(Name name, Slug slug, string? description, string definition)
    {
        Id = Id.NewId();
        Name = name;
        Slug = slug;
        Description = description;
        Definition = definition;
        Executions = [];
    }

    public static ErrorOr<Pipeline> Create(Name name, Slug slug, string? description, string definition)
    {
        return new Pipeline(name, slug, description, definition);
    }

    public ErrorOr<Success> SetName(Name name)
    {
        var slug = Slug.Create(name);
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

    public Name Name { get; private set; }

    public Slug Slug { get; private set; }

    public string? Description { get; private set; }

    public string Definition { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public ICollection<PipelineExecution> Executions { get; private set; }
}
