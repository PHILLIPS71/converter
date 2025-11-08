namespace Giantnodes.Infrastructure;

public interface ISpecificationComposition<TEntity> : ISpecification<TEntity>
    where TEntity : Entity
{
    ISpecification<TEntity> Left { get; }

    ISpecification<TEntity> Right { get; }
}
