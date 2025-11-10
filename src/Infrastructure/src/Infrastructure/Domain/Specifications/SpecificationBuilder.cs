namespace Giantnodes.Infrastructure;

public sealed class SpecificationBuilder<TEntity>
    where TEntity : IEntity
{
    private ISpecification<TEntity>? _specification;

    public SpecificationBuilder()
    {
    }

    /// <summary>
    /// Adds an And composition of the provided specifications.
    /// </summary>
    /// <param name="specifications">The specifications to combine with And logic.</param>
    public SpecificationBuilder<TEntity> And(params ISpecification<TEntity>[] specifications)
    {
        if (specifications.Length == 0)
            return this;

        ISpecification<TEntity>? combined = null;
        foreach (var specification in specifications)
        {
            combined = combined == null ? specification : combined.And(specification);
        }

        if (combined != null)
            _specification = _specification == null ? combined : _specification.And(combined);

        return this;
    }

    /// <summary>
    /// Adds an Or composition of the provided specifications.
    /// </summary>
    /// <param name="specifications">The specifications to combine with Or logic.</param>
    public SpecificationBuilder<TEntity> Or(params ISpecification<TEntity>[] specifications)
    {
        if (specifications.Length == 0)
            return this;

        ISpecification<TEntity>? combined = null;
        foreach (var specification in specifications)
        {
            combined = combined == null ? specification : combined.Or(specification);
        }

        if (combined != null)
            _specification = _specification == null ? combined : _specification.Or(combined);

        return this;
    }

    /// <summary>
    /// Adds an Any composition (at least one must match) of the provided specifications.
    /// </summary>
    /// <param name="specifications">The specifications where at least one must match.</param>
    public SpecificationBuilder<TEntity> Any(params ISpecification<TEntity>[] specifications)
    {
        if (specifications.Length == 0)
            return this;

        var combined = new AnySpecification<TEntity>(specifications);
        _specification = _specification == null ? combined : _specification.And(combined);
        return this;
    }

    /// <summary>
    /// Adds a None composition (none must match) of the provided specifications.
    /// </summary>
    /// <param name="specifications">The specifications where none must match.</param>
    public SpecificationBuilder<TEntity> None(params ISpecification<TEntity>[] specifications)
    {
        if (specifications.Length == 0)
            return this;

        var combined = new NoneSpecification<TEntity>(specifications);
        _specification = _specification == null ? combined : _specification.And(combined);
        return this;
    }

    /// <summary>
    /// Builds and returns the final composed specification.
    /// </summary>
    /// <returns>The composed specification.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no specifications have been added.</exception>
    public ISpecification<TEntity> Build()
    {
        if (_specification == null)
            throw new InvalidOperationException("cannot build specification without adding at least one specification");

        return _specification;
    }
}
