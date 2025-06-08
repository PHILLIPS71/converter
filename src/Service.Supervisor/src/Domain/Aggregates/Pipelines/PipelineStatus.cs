namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public enum PipelineStatus
{
    Submitted,
    Running,
    Cancelled,
    Failed,
    Completed
}