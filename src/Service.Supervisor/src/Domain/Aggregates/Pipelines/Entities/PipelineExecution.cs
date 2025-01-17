using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Infrastructure.Pipelines;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using HotChocolate;
using MassTransit;
using Error = ErrorOr.Error;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public sealed class PipelineExecution : Entity<Guid>, ITimestampableEntity
{
    private PipelineExecution()
    {
    }

    internal PipelineExecution(Pipeline pipeline, PipelineDefinition definition, FileSystemFile file)
    {
        Id = NewId.NextSequentialGuid();
        Pipeline = pipeline;
        Definition = definition;
        File = file;
        Context = new PipelineContext(new Dictionary<string, object>
        {
            { "__pipeline_id", pipeline.Id.ToString() },
            { "__pipeline_execution_id", Id.ToString() },
            { "path", file.PathInfo.FullName }
        });
    }

    public ErrorOr<Success> Start()
    {
        if (StartedAt.HasValue)
            return Error.Conflict("the execution has already started");

        if (Failure != null)
            return Error.Conflict("cannot start a failed execution");

        if (CompletedAt.HasValue)
            return Error.Conflict("cannot start a completed execution");

        StartedAt = DateTime.UtcNow;
        return Result.Success;
    }

    public ErrorOr<Success> Fail(string reason, DateTime? occurred = null)
    {
        if (Failure != null)
            return Error.Conflict("the execution has already failed");

        if (CompletedAt.HasValue)
            return Error.Conflict("cannot fail a completed execution");

        if (!StartedAt.HasValue)
            return Error.Conflict("cannot fail an execution that hasn't started");

        var result = PipelineFailure.Create(reason, occurred);
        if (result.IsError)
            return result.Errors;

        Failure = result.Value;
        return Result.Success;
    }

    public ErrorOr<Success> Complete(DateTime completed)
    {
        if (completed == default)
            return Error.Validation("a completion time is required");

        if (completed < StartedAt.GetValueOrDefault())
            return Error.Validation("the completion time cannot be before start time");

        if (CompletedAt.HasValue)
            return Error.Conflict("the execution has already completed");

        if (Failure != null)
            return Error.Conflict("cannot complete a failed execution");

        if (!StartedAt.HasValue)
            return Error.Conflict("cannot complete an execution that hasn't started");

        CompletedAt = completed;
        return Result.Success;
    }

    public Pipeline Pipeline { get; private set; }

    public PipelineDefinition Definition { get; private set; }

    [GraphQLIgnore] // https://github.com/ChilliCream/graphql-platform/issues/7170
    public PipelineContext Context { get; private set; }

    public FileSystemFile File { get; private set; }

    public PipelineFailure? Failure { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public PipelineStatus Status
    {
        get
        {
            if (Failure != null)
                return PipelineStatus.Failed;

            if (CompletedAt.HasValue)
                return PipelineStatus.Completed;

            if (StartedAt.HasValue)
                return PipelineStatus.Running;

            return PipelineStatus.Pending;
        }
    }

    public TimeSpan? Duration
    {
        get
        {
            if (!StartedAt.HasValue)
                return null;

            var end = CompletedAt ?? Failure?.FailedAt;
            if (!end.HasValue)
                return null;

            return end - StartedAt.Value;
        }
    }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
}