namespace Giantnodes.Infrastructure;

public static class SpecificationExtensions
{
    public static ISpecification<TEntity> And<TEntity>(
        this ISpecification<TEntity> specification,
        ISpecification<TEntity> other)
        where TEntity : IEntity
    {
        return new AndSpecification<TEntity>(specification, other);
    }

    public static ISpecification<TEntity> Or<TEntity>(
        this ISpecification<TEntity> specification,
        ISpecification<TEntity> other)
        where TEntity : IEntity
    {
        return new OrSpecification<TEntity>(specification, other);
    }

    public static ISpecification<TEntity> Any<TEntity>(
        params ISpecification<TEntity>[] specifications)
        where TEntity : IEntity
    {
        return new AnySpecification<TEntity>(specifications);
    }

    public static ISpecification<TEntity> None<TEntity>(
        params ISpecification<TEntity>[] specifications)
        where TEntity : IEntity
    {
        return new NoneSpecification<TEntity>(specifications);
    }
}
