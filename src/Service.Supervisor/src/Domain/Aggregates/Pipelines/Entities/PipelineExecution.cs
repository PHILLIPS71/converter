using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using MassTransit;

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

    public PipelineStatus Status { get; private set; } = PipelineStatus.Submitted;

    public TimeSpan? Duration
    {
        get
        {
            if (!StartedAt.HasValue)
                return null;

            var end = CompletedAt ?? CancelledAt ?? Failure?.FailedAt;
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
        if (Status != PipelineStatus.Submitted)
            return Error.Conflict($"cannot start execution in {Status} status");

        StartedAt = started;
        Status = PipelineStatus.Running;
        return Result.Success;
    }

    public ErrorOr<Success> Fail(string reason, DateTime? occurred = null)
    {
        if (Status is PipelineStatus.Failed or PipelineStatus.Cancelled or PipelineStatus.Completed)
            return Error.Conflict($"cannot fail execution in {Status} status");

        if (Status == PipelineStatus.Submitted)
            return Error.Conflict("cannot fail an execution that hasn't started");

        var result = PipelineFailure.Create(reason, occurred);
        if (result.IsError)
            return result.Errors;

        Failure = result.Value;
        Status = PipelineStatus.Failed;
        return Result.Success;
    }

    public ErrorOr<Success> Cancel(DateTime? occurred = null)
    {
        if (Status is PipelineStatus.Cancelled or PipelineStatus.Failed or PipelineStatus.Completed)
            return Error.Conflict($"cannot cancel execution in {Status} status");

        CancelledAt = occurred ?? DateTime.UtcNow;
        Status = PipelineStatus.Cancelled;
        return Result.Success;
    }

    public ErrorOr<Success> Complete(DateTime completed)
    {
        if (completed == default)
            return Error.Validation("a completion time is required");

        if (completed < StartedAt.GetValueOrDefault())
            return Error.Validation("the completion time cannot be before start time");

        if (Status != PipelineStatus.Running)
            return Error.Conflict($"cannot complete execution in {Status} status");

        CompletedAt = completed;
        Status = PipelineStatus.Completed;
        return Result.Success;
    }
}
