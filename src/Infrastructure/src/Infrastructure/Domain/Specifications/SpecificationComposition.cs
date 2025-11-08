namespace Giantnodes.Infrastructure;

public abstract class SpecificationComposition<TEntity> : Specification<TEntity>, ISpecificationComposition<TEntity>
    where TEntity : IEntity
{
    protected SpecificationComposition(ISpecification<TEntity> left, ISpecification<TEntity> right)
    {
        Left = left;
        Right = right;
    }

    public ISpecification<TEntity> Left { get; }

    public ISpecification<TEntity> Right { get; }
}
