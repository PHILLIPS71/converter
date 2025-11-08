namespace Giantnodes.Infrastructure;

public static class StringExtensions
{
    private const string DefaultConjunction = "and";
    private const string DefaultDelimiter = ", ";

    /// <summary>
    /// Formats a collection of strings into a grammatically correct list using the specified conjunction and delimiter.
    /// </summary>
    /// <param name="source">The collection to format. Cannot be null.</param>
    /// <param name="conjunction">The word to join the last two items. Defaults to "and".</param>
    /// <param name="delimiter">The delimiter to use between items. Defaults to ", ".</param>
    /// <returns>A formatted string representation of the collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static string ToGrammaticalSequence(
        this IEnumerable<string> source,
        string conjunction = DefaultConjunction,
        string delimiter = DefaultDelimiter)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(conjunction))
            conjunction = DefaultConjunction;

        if (string.IsNullOrWhiteSpace(delimiter))
            delimiter = DefaultDelimiter;

        var items = source
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToList();

        return items.Count switch
        {
            0 => string.Empty,
            1 => items[0],
            2 => $"{items[0]} {conjunction} {items[1]}",
            _ => $"{string.Join(delimiter, items.Take(items.Count - 1))} {conjunction} {items[^1]}"
        };
    }

    /// <summary>
    /// Formats a collection using an Oxford comma style (with comma before conjunction).
    /// </summary>
    /// <param name="source">The collection to format. Cannot be null.</param>
    /// <param name="conjunction">The word to join the last two items. Defaults to "and".</param>
    /// <returns>A formatted string representation of the collection using Oxford comma style.</returns>
    public static string ToOxfordSequence(
        this IEnumerable<string> source,
        string conjunction = DefaultConjunction)
        => source.ToGrammaticalSequence(conjunction: conjunction, delimiter: $"{DefaultDelimiter}");
}
