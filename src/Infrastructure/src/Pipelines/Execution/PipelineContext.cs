using System.Collections.Concurrent;
using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Provides a shared context for pipeline execution, containing state and step outputs that are accessible throughout
/// the pipeline execution lifecycle.
/// </summary>
public sealed class PipelineContext
{
    /// <summary>
    /// Gets the shared state dictionary that persists across all pipeline steps. This is the primary mechanism for
    /// passing data between steps.
    /// </summary>
    public IDictionary<string, object> State { get; init; }

    /// <summary>
    /// Gets the outputs produced by completed pipeline steps, indexed by step ID. Each step can produce multiple named
    /// outputs stored as key-value pairs.
    /// </summary>
    public IDictionary<string, IReadOnlyDictionary<string, object>> Outputs { get; init; }

    /// <summary>
    /// Initializes a new pipeline context with empty state and outputs.
    /// </summary>
    public PipelineContext()
    {
        State = new ConcurrentDictionary<string, object>();
        Outputs = new ConcurrentDictionary<string, IReadOnlyDictionary<string, object>>();
    }

    /// <summary>
    /// Initializes a new pipeline context with the specified initial state.
    /// </summary>
    /// <param name="state">The initial state values to populate the context with.</param>
    public PipelineContext(IDictionary<string, object> state)
    {
        State = new ConcurrentDictionary<string, object>(state);
        Outputs = new ConcurrentDictionary<string, IReadOnlyDictionary<string, object>>();
    }

    /// <summary>
    /// Stores the outputs produced by a pipeline step for later retrieval by subsequent steps.
    /// </summary>
    /// <param name="id">The unique identifier of the step that produced the outputs.</param>
    /// <param name="outputs">The collection of named output values produced by the step.</param>
    /// <returns>Success result, or a validation error if the step ID is invalid.</returns>
    public ErrorOr<Success> SetStepOutputs(string id, IReadOnlyDictionary<string, object> outputs)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Error.Validation("step id cannot be null, empty, or whitespace");

        Outputs[id] = outputs;
        return Result.Success;
    }

    /// <summary>
    /// Retrieves a strongly-typed output value from a completed pipeline step.
    /// </summary>
    /// <typeparam name="T">The expected type of the output value.</typeparam>
    /// <param name="id">The unique identifier of the step that produced the output.</param>
    /// <param name="name">The name of the output value to retrieve.</param>
    /// <returns>The typed output value, or an error if not found or type conversion fails.</returns>
    public ErrorOr<T> GetStepOutput<T>(string id, string name)
    {
        if (!Outputs.TryGetValue(id, out var outputs))
            return Error.NotFound($"no outputs found for step id '{id}'.");

        if (!outputs.TryGetValue(name, out var value))
            return Error.NotFound($"no output named '{name}' found for step id '{id}'.");

        if (value is not T typed)
            return Error.Validation($"output '{name}' for step id '{id}' is not of type {typeof(T).Name}.");

        return typed;
    }
}
