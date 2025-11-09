using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

/// <summary>
/// Value generator that generates new <see cref="Id"/> values using ULIDs.
/// </summary>
public sealed class IdValueGenerator : ValueGenerator<Id>
{
    public override Id Next(EntityEntry entry)
        => Id.NewId();

    public override bool GeneratesTemporaryValues => false;
}

/// <summary>
/// Value generator that generates new <see cref="Id{T}"/> values using ULIDs.
/// </summary>
/// <typeparam name="T">The entity type associated with the identifier.</typeparam>
public sealed class IdValueGenerator<T> : ValueGenerator<Id<T>>
{
    public override Id<T> Next(EntityEntry entry)
        => Id<T>.NewId();

    public override bool GeneratesTemporaryValues => false;
}
