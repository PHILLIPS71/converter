using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public class AndSpecification<TEntity> : SpecificationComposition<TEntity>
    where TEntity : Entity
{
    public AndSpecification(ISpecification<TEntity> left, ISpecification<TEntity> right)
        : base(left, right)
    {
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return Left.ToExpression().And(Right.ToExpression());
    }
}
