namespace Giantnodes.Infrastructure;

public static class OptionsExtensions
{
    /// <summary>
    /// Copies properties from the source options to the destination options.
    /// </summary>
    /// <typeparam name="T">The type of options.</typeparam>
    /// <param name="source">The source options.</param>
    /// <param name="destination">The destination options.</param>
    public static void CopyTo<T>(this T source, T destination) where T : class
    {
        foreach (var property in typeof(T).GetProperties())
        {
            if (property is { CanRead: true, CanWrite: true })
            {
                property.SetValue(destination, property.GetValue(source));
            }
        }
    }
}