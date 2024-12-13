using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public interface IPipelineSpecification
{
    string Name { get; }

    Task<ErrorOr<Success>> ExecuteAsync(
        PipelineSpecificationDefinition definition,
        PipelineContext context,
        CancellationToken cancellation = default);
}