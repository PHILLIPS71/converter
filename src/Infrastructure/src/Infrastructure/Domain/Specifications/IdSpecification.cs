using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

public sealed class IdSpecification<TEntity, TKey> : Specification<TEntity>
    where TEntity : IEntity<TKey>
    where TKey : notnull
{
    private readonly ICollection<TKey> _ids;

    public IdSpecification(ICollection<TKey> id)
    {
        _ids = id;
    }

    public IdSpecification(TKey id)
    {
        _ids = [id];
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return entity => _ids.Contains(entity.Id);
    }
}
