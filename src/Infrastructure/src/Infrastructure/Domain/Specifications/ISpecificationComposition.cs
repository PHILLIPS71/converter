namespace Giantnodes.Infrastructure;

public interface ISpecificationComposition<TEntity> : ISpecification<TEntity>
    where TEntity : IEntity
{
    public ISpecification<TEntity> Left { get; }

    public ISpecification<TEntity> Right { get; }
}
