using ErrorOr;
using Giantnodes.Infrastructure;
using Slugify;

namespace Giantnodes.Service.Supervisor.Domain.Aggregates.Libraries;

public sealed record LibrarySlug : ValueObject
{
    private LibrarySlug(string value) => Value = value;

    public string Value { get; }

    public static ErrorOr<LibrarySlug> Create(string value)
    {
        return new LibrarySlug(new SlugHelper().GenerateSlug(value));
    }

    public static ErrorOr<LibrarySlug> Create(LibraryName name)
    {
        return new LibrarySlug(new SlugHelper().GenerateSlug(name.Value));
    }
}
