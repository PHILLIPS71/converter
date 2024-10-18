using System.Linq.Expressions;
using System.Reflection;

namespace Giantnodes.Infrastructure.EntityFrameworkCore;

internal sealed class ObjectHelper
{
    /// <summary>
    /// Sets the value of a property on an object using an expression representing the property.
    /// </summary>
    /// <typeparam name="TObject">The type of the object containing the property.</typeparam>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    /// <param name="obj">The object whose property is to be set.</param>
    /// <param name="property">An expression representing the property to set.</param>
    /// <param name="value">A function to provide the new value for the property.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the property is not found in the object's type or is not writable.
    /// </exception>
    public static void SetProperty<TObject, TValue>(
        TObject obj,
        Expression<Func<TObject, TValue>> property,
        Func<TObject, TValue> value) where TObject : class
    {
        if (property.Body is not MemberExpression expression)
            return;

        const BindingFlags flags = BindingFlags.Instance |
                                   BindingFlags.Public |
                                   BindingFlags.NonPublic |
                                   BindingFlags.SetProperty;

        var parent = obj.GetType();
        while (parent != null)
        {
            var info = parent.GetProperty(expression.Member.Name, flags);
            if (info == null)
                throw new ArgumentOutOfRangeException(expression.Member.Name,
                    $"Property {expression.Member.Name} was not found in Type {parent.FullName}");

            if (info.CanWrite)
                break;

            parent = obj.GetType().BaseType;
        }

        parent?.InvokeMember(expression.Member.Name, flags, Type.DefaultBinder, obj, [value(obj)]);
    }
}