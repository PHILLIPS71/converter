using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public sealed class PipelineContext
{
    public IDictionary<string, object> State { get; init; }

    public IDictionary<string, IReadOnlyDictionary<string, object>> Outputs { get; init; }

    public PipelineContext()
    {
    }

    public PipelineContext(IDictionary<string, object> state)
    {
        State = new ConcurrentDictionary<string, object>(state);
        Outputs = new ConcurrentDictionary<string, IReadOnlyDictionary<string, object>>();
    }

    public ErrorOr<Success> SetStepOutputs(string id, IReadOnlyDictionary<string, object> outputs)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Error.Validation("step id cannot be null, empty, or whitespace");

        Outputs[id] = outputs;
        return Result.Success;
    }

    public ErrorOr<T> GetStepOutput<T>(string id, string name)
    {
        if (!Outputs.TryGetValue(id, out var outputs))
            return Error.NotFound($"no outputs found for step id '{id}'.");

        if (!outputs.TryGetValue(id, out var value))
            return Error.NotFound($"no output named '{name}' found for step id '{id}'.");

        if (value is not T typed)
            return Error.Validation($"output '{name}' for step id '{id}' is not of type {typeof(T).Name}.");

        return typed;
    }
}