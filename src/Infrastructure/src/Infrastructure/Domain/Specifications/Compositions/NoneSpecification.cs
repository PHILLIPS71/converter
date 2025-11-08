using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public class NoneSpecification<TEntity> : Specification<TEntity>
    where TEntity : IEntity
{
    private readonly ISpecification<TEntity>[] _specifications;

    public NoneSpecification(params ISpecification<TEntity>[] specifications)
    {
        _specifications = specifications;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        if (_specifications.Length == 0)
            return entity => false;

        // combine all specifications with OR
        var expression = _specifications[0].ToExpression();
        foreach (var specification in _specifications.Skip(1))
        {
            expression = expression.Or(specification.ToExpression());
        }

        // negate the entire expression (none should match)
        var parameter = expression.Parameters[0];
        var notBody = Expression.Not(expression.Body);
        return Expression.Lambda<Func<TEntity, bool>>(notBody, parameter);
    }
}
