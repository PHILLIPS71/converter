namespace Giantnodes.Infrastructure;

public static class AsyncEnumerableExtensions
{
    /// <summary>
    /// Converts an IAsyncEnumerable to a List asynchronously.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the sequence.</typeparam>
    /// <param name="elements">The IAsyncEnumerable to convert.</param>
    /// <returns>A Task representing the asynchronous operation, containing a List of the elements.</returns>
    public static async Task<IList<TElement>> ToListAsync<TElement>(this IAsyncEnumerable<TElement> elements)
        where TElement : class
    {
        var collection = new List<TElement>();
        await foreach (var element in elements)
            collection.Add(element);

        return collection;
    }

    /// <summary>
    /// Converts an IAsyncEnumerable to a List asynchronously, with support for cancellation.
    /// </summary>
    /// <typeparam name="TElement">The type of elements in the sequence.</typeparam>
    /// <param name="elements">The IAsyncEnumerable to convert.</param>
    /// <param name="cancellation">A CancellationToken that can be used to cancel the asynchronous operation.</param>
    /// <returns>A Task representing the asynchronous operation, containing a List of the elements.</returns>
    public static async Task<IList<TElement>> ToListAsync<TElement>(
        this IAsyncEnumerable<TElement> elements,
        CancellationToken cancellation)
        where TElement : class
    {
        var collection = new List<TElement>();
        await foreach (var element in elements.WithCancellation(cancellation))
            collection.Add(element);

        return collection;
    }
}
