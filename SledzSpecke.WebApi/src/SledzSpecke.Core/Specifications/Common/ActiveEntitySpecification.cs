using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Specifications.Common;

/// <summary>
/// Generic specification for filtering active entities
/// </summary>
public class ActiveEntitySpecification<T> : Specification<T> where T : class
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        // Check if entity has IsActive property
        var isActiveProperty = typeof(T).GetProperty("IsActive");
        if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, "IsActive");
            var constant = Expression.Constant(true);
            var equality = Expression.Equal(property, constant);
            
            return Expression.Lambda<Func<T, bool>>(equality, parameter);
        }

        // If no IsActive property, return all entities
        return entity => true;
    }
}