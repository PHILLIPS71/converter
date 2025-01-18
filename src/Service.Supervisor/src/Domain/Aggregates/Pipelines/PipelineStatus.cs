namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

public enum PipelineStatus
{
    Pending,
    Running,
    Failed,
    Completed
}