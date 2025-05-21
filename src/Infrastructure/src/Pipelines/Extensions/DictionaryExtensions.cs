using ErrorOr;

namespace Giantnodes.Infrastructure.Pipelines;

/// <summary>
/// Extension methods for <see cref="Dictionary{TKey, TValue}"/> with object values to provide type-safe access patterns.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Attempts to retrieve a value from the dictionary and cast it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the value to.</typeparam>
    /// <param name="dictionary">The dictionary to retrieve the value from.</param>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>The value cast to type <typeparamref name="T"/> if found and castable; otherwise, default value.</returns>
    public static ErrorOr<T?> GetOptional<T>(this IDictionary<string, object> dictionary, string key)
    {
        if (!dictionary.TryGetValue(key, out var value))
            return default;

        if (value is not T typed)
            return Error.Validation($"value for key '{key}' is not of type {typeof(T).Name}");

        return typed;
    }

    /// <summary>
    /// Attempts to retrieve a value from the dictionary and cast it to the specified type, returning a default value
    /// if not found.
    /// </summary>
    /// <typeparam name="T">The type to cast the value to.</typeparam>
    /// <param name="dictionary">The dictionary to retrieve the value from.</param>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The value cast to type <typeparamref name="T"/> if found and castable; otherwise, the default value.</returns>
    public static ErrorOr<T> GetOptional<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
    {
        if (!dictionary.TryGetValue(key, out var value))
            return defaultValue;

        if (value is not T typed)
            return Error.Validation($"value for key '{key}' is not of type {typeof(T).Name}");

        return typed;
    }

    /// <summary>
    /// Attempts to retrieve a value from the dictionary and convert it using a custom converter function.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="dictionary">The dictionary to retrieve the value from.</param>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="converter">A function to convert the object value to the desired type.</param>
    /// <returns>The converted value if found; otherwise, the default value.</returns>
    public static ErrorOr<T?> GetOptional<T>(
        this IDictionary<string, object> dictionary,
        string key,
        Func<object, ErrorOr<T?>> converter)
    {
        if (!dictionary.TryGetValue(key, out var value))
            return default;

        return converter(value);
    }

    /// <summary>
    /// Attempts to retrieve a value from the dictionary and convert it using a custom converter function, returning a
    /// default value if not found.
    /// </summary>
    /// <typeparam name="T">The type to convert the value to.</typeparam>
    /// <param name="dictionary">The dictionary to retrieve the value from.</param>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="converter">A function to convert the object value to the desired type.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The converted value if found; otherwise, the default value.</returns>
    public static ErrorOr<T> GetOptional<T>(
        this IDictionary<string, object> dictionary,
        string key,
        Func<object, ErrorOr<T>> converter, T defaultValue)
    {
        if (!dictionary.TryGetValue(key, out var value))
            return defaultValue;

        return converter(value);
    }

    /// <summary>
    /// Retrieves a value from the dictionary and casts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the value to.</typeparam>
    /// <param name="dictionary">The dictionary to retrieve the value from.</param>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>The value cast to type <typeparamref name="T"/> if found and castable; otherwise, an error.</returns>
    public static ErrorOr<T> Get<T>(this IDictionary<string, object> dictionary, string key)
    {
        if (!dictionary.TryGetValue(key, out var value))
            return Error.NotFound($"key '{key}' not found in dictionary");

        if (value is not T typed)
            return Error.Validation($"value for key '{key}' is not of type {typeof(T).Name}");

        return typed;
    }

    /// <summary>
    /// Attempts to retrieve and cast a value from the dictionary without throwing exceptions.
    /// </summary>
    /// <typeparam name="T">The type to cast the value to.</typeparam>
    /// <param name="dictionary">The dictionary to retrieve the value from.</param>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key if found and castable; otherwise, the default value.</param>
    /// <returns><c>true</c> if the value was successfully retrieved and cast; otherwise, <c>false</c>.</returns>
    public static bool TryGet<T>(this IDictionary<string, object> dictionary, string key, out T? value)
    {
        value = default;

        var result = dictionary.Get<T>(key);
        if (result.IsError)
            return false;

        value = result.Value;
        return true;
    }

    /// <summary>
    /// Determines whether the dictionary contains the specified key.
    /// </summary>
    /// <param name="dictionary">The dictionary to check.</param>
    /// <param name="key">The key to locate in the dictionary.</param>
    /// <returns><c>true</c> if the dictionary contains the specified key; otherwise, <c>false</c>.</returns>
    public static bool Has(this IDictionary<string, object> dictionary, string key) => dictionary.ContainsKey(key);
}