using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications.Common;

/// <summary>
/// Generic specification for filtering out soft-deleted entities
/// </summary>
public class NotDeletedSpecification<T> : Specification<T> where T : class
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        // Check for IsDeleted property
        var isDeletedProperty = typeof(T).GetProperty("IsDeleted");
        if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, "IsDeleted");
            var constant = Expression.Constant(false);
            var equality = Expression.Equal(property, constant);
            
            return Expression.Lambda<Func<T, bool>>(equality, parameter);
        }

        // Check for DeletedAt property
        var deletedAtProperty = typeof(T).GetProperty("DeletedAt");
        if (deletedAtProperty != null && 
            (deletedAtProperty.PropertyType == typeof(DateTime?) || 
             deletedAtProperty.PropertyType == typeof(DateTimeOffset?)))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, "DeletedAt");
            var nullConstant = Expression.Constant(null, deletedAtProperty.PropertyType);
            var equality = Expression.Equal(property, nullConstant);
            
            return Expression.Lambda<Func<T, bool>>(equality, parameter);
        }

        // If no deletion tracking properties, return all entities
        return entity => true;
    }
}