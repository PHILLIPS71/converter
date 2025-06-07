using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using MassTransit;
using Error = ErrorOr.Error;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public sealed class PipelineExecution : Entity<Guid>, ITimestampableEntity
{
    public Pipeline Pipeline { get; private set; }

    public FileSystemFile File { get; private set; }

    public string Definition { get; private set; }

    public PipelineFailure? Failure { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public DateTime? CancelledAt { get; private set; }

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

    private PipelineExecution()
    {
    }

    internal PipelineExecution(Pipeline pipeline, string definition, FileSystemFile file)
    {
        Id = NewId.NextSequentialGuid();
        Pipeline = pipeline;
        Definition = definition;
        File = file;
    }

    public ErrorOr<Success> Start(DateTime started)
    {
        if (StartedAt.HasValue)
            return Error.Conflict("the execution has already started");

        if (Failure != null)
            return Error.Conflict("cannot start a failed execution");

        if (CompletedAt.HasValue)
            return Error.Conflict("cannot start a completed execution");

        StartedAt = started;
        return Result.Success;
    }

    public ErrorOr<Success> Fail(string reason, DateTime? occurred = null)
    {
        if (Failure != null)
            return Error.Conflict("the execution has already failed");

        if (CancelledAt.HasValue)
            return Error.Conflict("the execution has already been cancelled");

        if (CompletedAt.HasValue)
            return Error.Conflict("cannot fail a cancelled execution");

        if (!StartedAt.HasValue)
            return Error.Conflict("cannot fail an execution that hasn't started");

        var result = PipelineFailure.Create(reason, occurred);
        if (result.IsError)
            return result.Errors;

        Failure = result.Value;
        return Result.Success;
    }

    public ErrorOr<Success> Cancel(DateTime? occurred = null)
    {
        if (CancelledAt.HasValue)
            return Error.Conflict("the execution has already been cancelled");

        if (Failure != null)
            return Error.Conflict("cannot cancel a failed execution");

        if (CompletedAt.HasValue)
            return Error.Conflict("cannot cancel a completed execution");

        CancelledAt = occurred;
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
        
        if (CancelledAt.HasValue)
            return Error.Conflict("cannot complete a cancelled execution");

        if (Failure != null)
            return Error.Conflict("cannot complete a failed execution");

        if (!StartedAt.HasValue)
            return Error.Conflict("cannot complete an execution that hasn't started");

        CompletedAt = completed;
        return Result.Success;
    }
}