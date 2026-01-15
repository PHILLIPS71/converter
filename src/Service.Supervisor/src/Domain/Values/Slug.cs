using ErrorOr;
using Giantnodes.Infrastructure;
using Slugify;

namespace Giantnodes.Service.Supervisor.Domain.Values;

/// <summary>
/// Represents a URL-friendly slug value object generated from a name or string.
/// </summary>
public sealed record Slug : ValueObject
{
    private Slug(string value) => Value = value;

    /// <summary>
    /// Gets the string value of the slug.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new Slug from a string value.
    /// </summary>
    /// <param name="value">The string value to convert to a slug.</param>
    /// <returns>An ErrorOr containing a valid Slug.</returns>
    public static ErrorOr<Slug> Create(string value)
    {
        return new Slug(new SlugHelper().GenerateSlug(value));
    }

    /// <summary>
    /// Creates a new Slug from a Name value object.
    /// </summary>
    /// <param name="name">The Name to convert to a slug.</param>
    /// <returns>An ErrorOr containing a valid Slug.</returns>
    public static ErrorOr<Slug> Create(Name name)
    {
        return new Slug(new SlugHelper().GenerateSlug(name.Value));
    }
}
