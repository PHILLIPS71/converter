using ErrorOr;
using MassTransit;

namespace Giantnodes.Infrastructure.Pipelines;

public sealed record PipelineStepDefinition
{
    /// <summary>
    /// The unique identifier of the step within a pipeline stage
    /// </summary>
    public string Id { get; init; } = NewId.NextSequentialGuid().ToString();

    /// <summary>
    /// The name of the step
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The type of operation to use
    /// </summary>
    public string Uses { get; init; } = string.Empty;

    /// <summary>
    /// The configuration properties for the step
    /// </summary>
    public Dictionary<string, object> With { get; init; } = [];

    public ErrorOr<T?> GetOptional<T>(string key)
    {
        if (!With.TryGetValue(key, out var value))
            return default;

        if (value is not T typed)
            return Error.Validation($"value for key '{key}' is not of type {typeof(T).Name}");

        return typed;
    }

    public ErrorOr<T> GetOptional<T>(string key, T defaultValue)
    {
        if (!With.TryGetValue(key, out var value))
            return defaultValue;

        if (value is not T typed)
            return Error.Validation($"value for key '{key}' is not of type {typeof(T).Name}");

        return typed;
    }

    public ErrorOr<T?> GetOptional<T>(string key, Func<object, ErrorOr<T?>> converter)
    {
        if (!With.TryGetValue(key, out var value))
            return default;

        return converter(value);
    }

    public ErrorOr<T> GetOptional<T>(string key, Func<object, ErrorOr<T>> converter, T defaultValue)
    {
        if (!With.TryGetValue(key, out var value))
            return defaultValue;

        return converter(value);
    }

    public ErrorOr<T> Get<T>(string key)
    {
        if (!With.TryGetValue(key, out var value))
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

    public bool Has(string key) => With.ContainsKey(key);
}