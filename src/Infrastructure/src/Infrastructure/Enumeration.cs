using System.Reflection;

namespace Giantnodes.Infrastructure;

/// <summary>
/// Abstract base record for enumerations with an integer identifier and a name.
/// </summary>
public abstract record Enumeration
{
    /// <summary>
    /// Gets the identifier of the enumeration.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the name of the enumeration.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumeration"/> class.
    /// </summary>
    /// <param name="id">The identifier of the enumeration.</param>
    /// <param name="name">The name of the enumeration.</param>
    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <inheritdoc/>
    public sealed override string ToString() => Name;

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Gets all values of the enumeration type.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <returns>An IEnumerable containing all values of the enumeration type.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();
    }

    /// <summary>
    /// Parses an enumeration based on a predicate.
    /// </summary>
    /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
    /// <param name="predicate">The predicate to match against enumeration items.</param>
    /// <returns>The matching enumeration item.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided reference does not match any enumeration value.</exception>
    public static TEnumeration Parse<TEnumeration>(Func<TEnumeration, bool> predicate)
        where TEnumeration : Enumeration
    {
        var match = GetAll<TEnumeration>().FirstOrDefault(predicate);
        if (match == null)
            throw new ArgumentException($"The provided predicate did not match any value in {typeof(TEnumeration)}.",
                nameof(predicate));

        return match;
    }

    /// <summary>
    /// Parses an enumeration value based on its name or identifier value.
    /// </summary>
    /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
    /// <param name="reference">The name or identifier value to parse.</param>
    /// <returns>The matching enumeration value.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided reference does not match any enumeration value.</exception>
    public static TEnumeration ParseByValueOrName<TEnumeration>(string reference)
        where TEnumeration : Enumeration
    {
        return Parse<TEnumeration>(item => item.Name == reference || item.Id.ToString() == reference);
    }

    /// <summary>
    /// Parses an enumeration based on a predicate.
    /// </summary>
    /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
    /// <param name="predicate">The predicate to match against enumeration items.</param>
    /// <param name="enumeration">Outputs the enumeration value if the string was successfully parsed; otherwise, the default value for the enumeration type.</param>
    /// <returns>The matching enumeration item.</returns>
    /// <returns>True if a match was successfully found; otherwise, false.</returns>
    public static bool TryParse<TEnumeration>(Func<TEnumeration, bool> predicate, out TEnumeration? enumeration)
        where TEnumeration : Enumeration
    {
        try
        {
            enumeration = Parse(predicate);
            return true;
        }
        catch (ArgumentException)
        {
            enumeration = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse an enumeration value based on its identifier.
    /// </summary>
    /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
    /// <param name="id">The identifier to parse.</param>
    /// <param name="enumeration">Outputs the enumeration value if the string was successfully parsed; otherwise, the default value for the enumeration type.</param>
    /// <returns>True if the string was successfully parsed; otherwise, false.</returns>
    public static bool TryParse<TEnumeration>(int id, out TEnumeration? enumeration)
        where TEnumeration : Enumeration
    {
        try
        {
            enumeration = Parse<TEnumeration>(item => item.Id == id);
            return true;
        }
        catch (ArgumentException)
        {
            enumeration = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse an enumeration value based on its name.
    /// </summary>
    /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
    /// <param name="name">The name to parse.</param>
    /// <param name="enumeration">outputs the enumeration value if the string was successfully parsed; otherwise, null.</param>
    /// <returns>True if the string was successfully parsed; otherwise, false.</returns>
    public static bool TryParse<TEnumeration>(string name, out TEnumeration? enumeration)
        where TEnumeration : Enumeration
    {
        try
        {
            enumeration = Parse<TEnumeration>(item => item.Name == name);
            return true;
        }
        catch (ArgumentException)
        {
            enumeration = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse an enumeration based on its id or name.
    /// </summary>
    /// <typeparam name="TEnumeration">The type of the enumeration.</typeparam>
    /// <param name="reference">The id or name to try parse.</param>
    /// <returns>The parsed enumeration item, or null if parsing fails.</returns>
    public static TEnumeration? TryParseByValueOrName<TEnumeration>(string reference)
        where TEnumeration : Enumeration
    {
        try
        {
            return Parse<TEnumeration>(item => item.Name == reference || item.Id.ToString() == reference);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}