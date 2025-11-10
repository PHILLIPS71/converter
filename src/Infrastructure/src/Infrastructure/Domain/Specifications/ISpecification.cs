using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public interface ISpecification<TEntity>
    where TEntity : IEntity
{
    public bool IsSatisfiedBy(TEntity entity);

    public Expression<Func<TEntity, bool>> ToExpression();
}
