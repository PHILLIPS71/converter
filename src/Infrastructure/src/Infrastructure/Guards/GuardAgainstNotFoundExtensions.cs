using Giantnodes.Infrastructure;

namespace Ardalis.GuardClauses;

public static class GuardAgainstNotFoundExtensions
{
    /// <summary>
    /// Throws a <see cref="NotFoundException"/> if the specified enumeration value is not found in the given collection.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="guard">The guard clause instance.</param>
    /// <param name="collection">The collection to search for the enumeration value.</param>
    /// <param name="input">The enumeration value to check for.</param>
    /// <returns>The input value if it is found in the collection.</returns>
    /// <exception cref="NotFoundException">Thrown if the input value is not found in the collection.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the input value is null.</exception>
    public static T NotFound<T>(this IGuardClause guard, IEnumerable<T> collection, T input)
        where T : Enumeration
    {
        guard.Null(input);

        if (!collection.Contains(input))
            throw new NotFoundException(input.Id.ToString(), nameof(collection));

        return input;
    }
}