namespace Giantnodes.Infrastructure;

public static class SpecificationExtensions
{
    public static ISpecification<TEntity> And<TEntity>(
        this ISpecification<TEntity> specification,
        ISpecification<TEntity> other)
        where TEntity : Entity
    {
        return new AndSpecification<TEntity>(specification, other);
    }

    public static ISpecification<TEntity> Or<TEntity>(
        this ISpecification<TEntity> specification,
        ISpecification<TEntity> other)
        where TEntity : Entity
    {
        return new OrSpecification<TEntity>(specification, other);
    }
}
