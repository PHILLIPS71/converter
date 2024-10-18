using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public class OrSpecification<TEntity> : SpecificationComposition<TEntity>
    where TEntity : Entity
{
    public OrSpecification(ISpecification<TEntity> left, ISpecification<TEntity> right)
        : base(left, right)
    {
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return Left.ToExpression().Or(Right.ToExpression());
    }
}