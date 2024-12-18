using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public class PipelineContext
{
    public IDictionary<string, object> State { get; private set; }

    public PipelineContext()
    {
        State = new Dictionary<string, object>();
    }

    public PipelineContext(IDictionary<string, object> state)
    {
        State = state;
    }

    public ErrorOr<T> Get<T>(string key)
    {
        if (!State.TryGetValue(key, out var value))
            return Error.NotFound($"key '{key}' not found in context");

        if (value is not T typed)
            return Error.Validation($"value for key '{key}' is not of type {typeof(T).Name}");

        return typed;
    }

    public bool TryGet<T>(string key, out T? value)
    {
        value = default;

        var result = Get<T>(key);
        if (result.IsError)
            return false;

        value = result.Value;
        return true;
    }

    public ErrorOr<Success> Set<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Error.Validation("key cannot be null or empty");

        if (value is null)
            return Error.Validation($"value for key '{key}' cannot be null");

        State[key] = value;
        return Result.Success;
    }

    public ErrorOr<Success> Remove(string key)
    {
        if (!State.ContainsKey(key))
            return Error.NotFound($"key '{key}' not found in context");

        State.Remove(key);
        return Result.Success;
    }

    public bool Has(string key) => State.ContainsKey(key);
}