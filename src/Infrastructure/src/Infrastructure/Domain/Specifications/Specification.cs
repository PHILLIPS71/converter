using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public abstract class Specification<TEntity> : ISpecification<TEntity>
    where TEntity : Entity
{
    public virtual bool IsSatisfiedBy(TEntity entity)
    {
        return ToExpression().Compile()(entity);
    }

    public abstract Expression<Func<TEntity, bool>> ToExpression();
}