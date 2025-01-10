using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using MassTransit;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public sealed class PipelineExecution : Entity<Guid>, ITimestampableEntity
{
    private PipelineExecution()
    {
    }

    public PipelineExecution(Pipeline pipeline)
    {
        Id = NewId.NextSequentialGuid();
        Pipeline = pipeline;
    }

    public Pipeline Pipeline { get; private set; }

    public PipelineDefinition Definition { get; private set; }

    public PipelineContext Context { get; private set; }

    public FileSystemFile File { get; private set; }

    public DateTime StartedAt { get; private set; }

    public DateTime CompletedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}