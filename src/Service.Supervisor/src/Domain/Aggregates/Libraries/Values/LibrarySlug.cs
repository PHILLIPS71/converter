using ErrorOr;
using Giantnodes.Infrastructure;
using Slugify;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public sealed record LibrarySlug : ValueObject
{
    private LibrarySlug(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ErrorOr<LibrarySlug> Create(string value)
    {
        var slug = new SlugHelper().GenerateSlug(value);
        return new LibrarySlug(slug);
    }

    public static ErrorOr<LibrarySlug> Create(LibraryName name)
    {
        var slug = new SlugHelper().GenerateSlug(name.Value);
        return new LibrarySlug(slug);
    }
}