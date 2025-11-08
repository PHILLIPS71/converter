using ErrorOr;
using Giantnodes.Infrastructure;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Entries.Files;
using Giantnodes.Service.Supervisor.Domain.Aggregates.Pipelines;

namespace Giantnodes.Service.Supervisor.Infrastructure.Services;

public interface IPipelineExecutionService : IApplicationService
{
    Task<ErrorOr<PipelineExecution>> ExecuteAsync(
        Pipeline pipeline,
        FileSystemFile file,
        CancellationToken cancellation = default);
}
