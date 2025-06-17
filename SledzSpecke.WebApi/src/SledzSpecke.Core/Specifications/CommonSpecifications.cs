using SledzSpecke.Core.Abstractions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Specifications;

/// <summary>
/// Generic specification for filtering entities by a specific ID
/// </summary>
public class EntityByIdSpecification<TEntity, TId> : Specification<TEntity>
    where TEntity : class
{
    private readonly TId _id;
    private readonly Func<TEntity, TId> _idSelector;

    public EntityByIdSpecification(TId id, Func<TEntity, TId> idSelector)
    {
        _id = id;
        _idSelector = idSelector;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return entity => entity != null && EqualityComparer<TId>.Default.Equals(_idSelector(entity), _id);
    }
}

/// <summary>
/// Generic specification for filtering entities within a date range
/// </summary>
public class EntityInDateRangeSpecification<TEntity> : Specification<TEntity>
    where TEntity : class
{
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;
    private readonly Func<TEntity, DateTime> _dateSelector;

    public EntityInDateRangeSpecification(
        DateTime startDate, 
        DateTime endDate, 
        Func<TEntity, DateTime> dateSelector)
    {
        _startDate = startDate;
        _endDate = endDate;
        _dateSelector = dateSelector;
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return entity => _dateSelector(entity) >= _startDate && _dateSelector(entity) <= _endDate;
    }
}

/// <summary>
/// Generic specification for pagination
/// </summary>
public class PaginationSpecification<TEntity> : Specification<TEntity>
    where TEntity : class
{
    private readonly int _skip;
    private readonly int _take;

    public PaginationSpecification(int pageNumber, int pageSize)
    {
        _skip = (pageNumber - 1) * pageSize;
        _take = pageSize;
    }

    public int Skip => _skip;
    public int Take => _take;

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        // Pagination is not expressed as a filter, handled separately
        return entity => true;
    }
}

/// <summary>
/// Generic specification for ordering
/// </summary>
public class OrderBySpecification<TEntity, TKey> : Specification<TEntity>
    where TEntity : class
{
    private readonly Expression<Func<TEntity, TKey>> _orderByExpression;
    private readonly bool _ascending;

    public OrderBySpecification(Expression<Func<TEntity, TKey>> orderByExpression, bool ascending = true)
    {
        _orderByExpression = orderByExpression;
        _ascending = ascending;
    }

    public Expression<Func<TEntity, TKey>> OrderByExpression => _orderByExpression;
    public bool Ascending => _ascending;

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        // Ordering is not expressed as a filter, handled separately
        return entity => true;
    }
}