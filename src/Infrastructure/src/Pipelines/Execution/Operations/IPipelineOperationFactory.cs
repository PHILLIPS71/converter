using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Defines the contract for factories responsible for creating pipeline operation instances based on their registered
/// names. This enables dynamic operation resolution at runtime.
/// </summary>
public interface IPipelineOperationFactory
{
    /// <summary>
    /// Creates a pipeline operation instance for the specified operation name.
    /// </summary>
    /// <param name="name">
    /// The unique name identifier of the operation to create. This should match
    /// the <see cref="IPipelineOperation.Name"/> property of a registered operation.
    /// </param>
    /// <returns>
    /// An <see cref="ErrorOr{T}"/> containing either the resolved operation instance  or an error if no operation is
    /// registered with the specified name.
    /// </returns>
    /// <remarks>
    /// The factory should maintain a registry of available operations and their implementations. Operations are
    /// typically registered during application startup through dependency injection.
    /// </remarks>
    ErrorOr<IPipelineOperation> Create(string name);
}
