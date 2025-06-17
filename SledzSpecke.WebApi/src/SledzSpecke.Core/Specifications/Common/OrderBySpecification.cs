using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Specifications.Common;

/// <summary>
/// Generic specification for ordering query results
/// </summary>
public class OrderBySpecification<T> : Specification<T> where T : class
{
    public Expression<Func<T, object>>? OrderByExpression { get; private set; }
    public Expression<Func<T, object>>? OrderByDescendingExpression { get; private set; }
    public bool IsDescending { get; private set; }

    public OrderBySpecification<T> OrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderByExpression = orderByExpression;
        IsDescending = false;
        return this;
    }

    public OrderBySpecification<T> OrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescendingExpression = orderByDescendingExpression;
        IsDescending = true;
        return this;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        // This specification doesn't filter, it only provides ordering
        return entity => true;
    }
}