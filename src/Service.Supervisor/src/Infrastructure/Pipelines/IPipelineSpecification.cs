using ErrorOr;

namespace Giantnodes.Service.Supervisor.Infrastructure.Pipelines;

public interface IPipelineSpecification
{
    string Name { get; }

    ErrorOr<Success> Configure(IDictionary<string, object>? inputs);
}

internal interface IPipelineSpecification<TContext> : IPipelineSpecification
    where TContext : PipelineContext
{
    Task<ErrorOr<TContext>> ExecuteAsync(TContext context, CancellationToken cancellation = default);
}