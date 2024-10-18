namespace Giantnodes.Infrastructure;

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects#value-object-implementation-in-c" />
/// </summary>
public abstract class ValueObject : IValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    private static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null)) 
            return false;

        return ReferenceEquals(left, null) || left.Equals(right);
    }

    private static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !EqualOperator(left, right);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x.GetHashCode())
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return NotEqualOperator(left, right);
    }
}