using System.Linq.Expressions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Specifications.Common;

/// <summary>
/// Generic specification for pagination
/// </summary>
public class PaginationSpecification<T> : Specification<T> where T : class
{
    public int Skip { get; }
    public int Take { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public PaginationSpecification(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber > 0 ? pageNumber : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
        
        // Ensure reasonable limits
        if (PageSize > 100)
            PageSize = 100;
            
        Skip = (PageNumber - 1) * PageSize;
        Take = PageSize;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        // This specification doesn't filter, it only provides pagination parameters
        return entity => true;
    }
}