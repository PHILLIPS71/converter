using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public class AnySpecification<TEntity> : Specification<TEntity>
    where TEntity : Entity
{
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return entity => true;
    }
}