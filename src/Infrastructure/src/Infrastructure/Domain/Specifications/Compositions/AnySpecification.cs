using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public class AnySpecification<TEntity> : Specification<TEntity>
    where TEntity : IEntity
{
    private readonly ISpecification<TEntity>[] _specifications;

    public AnySpecification(params ISpecification<TEntity>[] specifications)
    {
        _specifications = specifications;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        if (_specifications.Length == 0)
            return entity => true;

        var expression = _specifications[0].ToExpression();
        foreach (var specification in _specifications.Skip(1))
        {
            expression = expression.Or(specification.ToExpression());
        }

        return expression;
    }
}
