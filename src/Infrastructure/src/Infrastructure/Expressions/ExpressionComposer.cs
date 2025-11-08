using System.Linq.Expressions;

namespace Giantnodes.Infrastructure;

// https://stackoverflow.com/questions/457316/combining-two-expressions-expressionfunct-bool
public static class ExpressionComposer
{
    /// <summary>
    /// Combines two expressions using the logical AND operator.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expressions.</typeparam>
    /// <param name="left">The left expression.</param>
    /// <param name="right">The right expression.</param>
    /// <returns>A new expression representing the logical AND combination of the input expressions.</returns>
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        return Compose(left, right, Expression.And);
    }

    /// <summary>
    /// Combines two expressions using the logical OR operator.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expressions.</typeparam>
    /// <param name="left">The left expression.</param>
    /// <param name="right">The right expression.</param>
    /// <returns>A new expression representing the logical OR combination of the input expressions.</returns>
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        return Compose(left, right, Expression.Or);
    }

    /// <summary>
    /// Composes two expressions using the specified binary operator.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expressions.</typeparam>
    /// <param name="left">The left expression.</param>
    /// <param name="right">The right expression.</param>
    /// <param name="operator">The binary operator to use for composition (AND or OR).</param>
    /// <returns>A new expression representing the composition of the input expressions.</returns>
    private static Expression<Func<T, bool>> Compose<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, BinaryExpression> @operator)
    {
        var leftParameter = left.Parameters[0];
        var rightParameter = right.Parameters[0];

        var visitor = new ReplaceParameterVisitor(rightParameter, leftParameter);

        return Expression.Lambda<Func<T, bool>>(@operator(left.Body, visitor.Visit(right.Body)), leftParameter);
    }

    /// <summary>
    /// Expression visitor that replaces occurrences of a specified parameter with another parameter.
    /// </summary>
    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _old;
        private readonly ParameterExpression _new;

        public ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _old = oldParameter;
            _new = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, _old) ? _new : base.VisitParameter(node);
        }
    }
}
