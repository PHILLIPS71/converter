namespace Giantnodes.Infrastructure;

public readonly record struct Id(Ulid Value) : IId, IComparable<Id>
{
    public Id(string value) : this(Ulid.Parse(value))
    {
    }

    public Id() : this(Ulid.Empty)
    {
    }

    public Id(Guid guid) : this(new Ulid(guid))
    {
    }

    public Id(Id id) : this(id.Value)
    {
    }

    public bool HasValue => Value != Ulid.Empty;

    public override string ToString()
        => Value.ToString();

    public int CompareTo(Id other)
        => Value.CompareTo(other.Value);

    public static Id NewId()
        => new(Ulid.NewUlid());

    public static Id Parse(ReadOnlySpan<byte> base32)
        => new(Ulid.Parse(base32));

    public static Id Parse(string value)
        => new(Ulid.Parse(value));

    public static bool TryParse(ReadOnlySpan<byte> base32, out Id id)
    {
        if (Ulid.TryParse(base32, out var ulid))
        {
            id = new Id(ulid);
            return true;
        }

        id = default;
        return false;
    }

    public static bool TryParse(string input, out Id id)
    {
        if (Ulid.TryParse(input, out var ulid))
        {
            id = new Id(ulid);
            return true;
        }

        id = default;
        return false;
    }
}

public readonly record struct Id<T>(Ulid Value) : IId, IComparable<Id<T>>, IComparable<Id>
{
    public Id(string value) : this(Ulid.Parse(value))
    {
    }

    public Id() : this(Ulid.Empty)
    {
    }

    public Id(Guid guid) : this(new Ulid(guid))
    {
    }

    public Id(Id id) : this(id.Value)
    {
    }

    public Id(Id<T> id) : this(id.Value)
    {
    }

    public bool HasValue => Value != Ulid.Empty;

    public override string ToString()
        => Value.ToString();

    public int CompareTo(Id<T> other)
        => Value.CompareTo(other.Value);

    public int CompareTo(Id other)
        => Value.CompareTo(other.Value);

    public static Id<T> NewId()
        => new(Ulid.NewUlid());

    public static Id<T> Parse(ReadOnlySpan<byte> base32)
        => new(Ulid.Parse(base32));

    public static Id<T> Parse(string value)
        => new(Ulid.Parse(value));

    public static bool TryParse(ReadOnlySpan<byte> base32, out Id<T> id)
    {
        if (Ulid.TryParse(base32, out var ulid))
        {
            id = new Id<T>(ulid);
            return true;
        }

        id = default;
        return false;
    }

    public static bool TryParse(string input, out Id id)
    {
        if (Ulid.TryParse(input, out var ulid))
        {
            id = new Id(ulid);
            return true;
        }

        id = default;
        return false;
    }

    public Id ToId() => new(Value);

    public static Id<T> FromId(Id id) => new(id.Value);

    public static implicit operator Id(Id<T> id) => id.ToId();
    public static implicit operator Id<T>(Id id) => FromId(id);

    public static bool operator ==(Id<T> left, Id right) => left.Value == right.Value;
    public static bool operator !=(Id<T> left, Id right) => !(left == right);

    public static bool operator ==(Id left, Id<T> right) => left.Value == right.Value;
    public static bool operator !=(Id left, Id<T> right) => !(left == right);
}
