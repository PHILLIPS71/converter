using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public sealed class IdSpecification<TEntity, TKey> : Specification<TEntity>
    where TEntity : IEntity<TKey>
    where TKey : notnull
{
    private readonly TKey _id;

    public IdSpecification(TKey id)
    {
        _id = id;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return entity => entity.Id.Equals(_id);
    }
}
