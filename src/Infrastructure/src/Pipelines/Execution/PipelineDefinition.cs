using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineDefinition
{
    public required string Name { get; init; }

    public string? Description { get; init; }

    public ICollection<PipelineSpecificationDefinition> Specifications { get; init; } = [];
}

public sealed record PipelineSpecificationDefinition
{
    public required string Name { get; init; }

    public required string Uses { get; init; }

    public required IDictionary<string, object> Properties { get; init; } = new Dictionary<string, object>();

    public ErrorOr<T?> GetOptional<T>(string key)
    {
        if (!Properties.TryGetValue(key, out var value))
            return default;

        if (value is not T typed)
            return Error.Validation($"value for key '{key}' is not of type {typeof(T).Name}");

        return typed;
    }

    public ErrorOr<T> GetOptional<T>(string key, T defaultValue)
    {
        if (!Properties.TryGetValue(key, out var value))
            return defaultValue;

        if (value is not T typed)
            return Error.Validation($"value for key '{key}' is not of type {typeof(T).Name}");

        return typed;
    }

    public ErrorOr<T?> GetOptional<T>(string key, Func<object, ErrorOr<T?>> converter)
    {
        if (!Properties.TryGetValue(key, out var value))
            return default;

        return converter(value);
    }

    public ErrorOr<T> GetOptional<T>(string key, Func<object, ErrorOr<T>> converter, T defaultValue)
    {
        if (!Properties.TryGetValue(key, out var value))
            return defaultValue;

        return converter(value);
    }

    public ErrorOr<T> Get<T>(string key)
    {
        if (!Properties.TryGetValue(key, out var value))
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

    public bool Has(string key) => Properties.ContainsKey(key);
}