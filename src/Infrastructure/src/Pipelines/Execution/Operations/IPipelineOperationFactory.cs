using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Interface for a pipeline operation factory to create operation executables.
/// </summary>
public interface IPipelineOperationFactory
{
    /// <summary>
    /// Creates an operation executable based on the specified name.
    /// </summary>
    /// <param name="name">The name of a operation to create.</param>
    /// <returns>A operation executable or error if creation fails.</returns>
    ErrorOr<IPipelineOperation> Create(string name);
}