using SledzSpecke.Core.Abstractions;
using System.Linq.Expressions;

namespace SledzSpecke.Core.Repositories;

/// <summary>
/// Generic repository interface for common data access operations
/// </summary>
public interface IGenericRepository<TEntity> where TEntity : class
{
    // Basic CRUD operations
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);

    // Specification-based queries
    Task<IEnumerable<TEntity>> GetBySpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);
    
    Task<TEntity?> GetSingleBySpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    // Existence and count operations
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
    
    Task<int> CountBySpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    // Pagination support
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        ISpecification<TEntity> specification,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Extension of generic repository for entities with specific ID types
/// </summary>
public interface IGenericRepository<TEntity, TId> : IGenericRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}