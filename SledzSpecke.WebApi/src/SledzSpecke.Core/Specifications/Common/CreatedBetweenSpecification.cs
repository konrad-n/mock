using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications.Common;

/// <summary>
/// Generic specification for filtering entities by creation date range
/// </summary>
public class CreatedBetweenSpecification<T> : Specification<T> where T : class
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;

    public CreatedBetweenSpecification(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var createdAtProperty = typeof(T).GetProperty("CreatedAt");
        if (createdAtProperty != null && createdAtProperty.PropertyType == typeof(DateTime))
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, "CreatedAt");
            var startConstant = Expression.Constant(_startDate);
            var endConstant = Expression.Constant(_endDate);
            
            var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, startConstant);
            var lessThanOrEqual = Expression.LessThanOrEqual(property, endConstant);
            var andExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
            
            return Expression.Lambda<Func<T, bool>>(andExpression, parameter);
        }

        // If no CreatedAt property, return all entities
        return entity => true;
    }
}