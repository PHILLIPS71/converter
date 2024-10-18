using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public interface ISpecification<TEntity> 
    where TEntity : Entity
{
    bool IsSatisfiedBy(TEntity entity);

    Expression<Func<TEntity, bool>> ToExpression();
}