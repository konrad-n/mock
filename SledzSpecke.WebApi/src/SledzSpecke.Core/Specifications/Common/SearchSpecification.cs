using System.Linq.Expressions;
using System.Reflection;

namespace SledzSpecke.Core.Specifications.Common;

/// <summary>
/// Generic specification for text search across multiple properties
/// </summary>
public class SearchSpecification<T> : Specification<T> where T : class
{
    private readonly string _searchTerm;
    private readonly string[] _propertiesToSearch;

    public SearchSpecification(string searchTerm, params string[] propertiesToSearch)
    {
        _searchTerm = searchTerm?.ToLower() ?? string.Empty;
        _propertiesToSearch = propertiesToSearch;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        if (string.IsNullOrWhiteSpace(_searchTerm))
            return entity => true;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var propertyName in _propertiesToSearch)
        {
            var property = typeof(T).GetProperty(propertyName);
            if (property != null && property.PropertyType == typeof(string))
            {
                // x.PropertyName
                var propertyAccess = Expression.Property(parameter, property);
                
                // x.PropertyName != null
                var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                
                // x.PropertyName.ToLower()
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var toLowerCall = Expression.Call(propertyAccess, toLowerMethod!);
                
                // x.PropertyName.ToLower().Contains(searchTerm)
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsCall = Expression.Call(toLowerCall, containsMethod!, Expression.Constant(_searchTerm));
                
                // x.PropertyName != null && x.PropertyName.ToLower().Contains(searchTerm)
                var condition = Expression.AndAlso(nullCheck, containsCall);

                if (combinedExpression == null)
                {
                    combinedExpression = condition;
                }
                else
                {
                    // OR with previous conditions
                    combinedExpression = Expression.OrElse(combinedExpression, condition);
                }
            }
        }

        if (combinedExpression == null)
            return entity => true;

        return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
    }
}