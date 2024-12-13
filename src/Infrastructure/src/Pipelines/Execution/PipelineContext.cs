using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public class PipelineContext
{
    private readonly Dictionary<string, object> _state;

    public PipelineContext()
    {
        _state = new Dictionary<string, object>();
    }

    public ErrorOr<T> Get<T>(string key)
    {
        if (!_state.TryGetValue(key, out var value))
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

        _state[key] = value;
        return Result.Success;
    }

    public ErrorOr<Success> Remove(string key)
    {
        if (!_state.ContainsKey(key))
            return Error.NotFound($"key '{key}' not found in context");

        _state.Remove(key);
        return Result.Success;
    }

    public bool Has(string key) => _state.ContainsKey(key);

    public IReadOnlyDictionary<string, object> GetState() => _state;
}