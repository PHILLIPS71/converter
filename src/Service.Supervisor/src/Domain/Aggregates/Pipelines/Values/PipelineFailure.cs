using ErrorOr;
using Giantnodes.Infrastructure;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public sealed record PipelineFailure : ValueObject
{
    private PipelineFailure()
    {
    }

    private PipelineFailure(string reason, DateTime occurred)
    {
        Reason = reason;
        FailedAt = occurred;
    }

    public static ErrorOr<PipelineFailure> Create(string reason, DateTime? occurred = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Error.Validation("a pipeline failure reason cannot be empty");

        reason = reason.Trim();
        return new PipelineFailure(reason, occurred ?? DateTime.UtcNow);
    }

    public string Reason { get; private set; }

    public DateTime FailedAt { get; private set; }
}
