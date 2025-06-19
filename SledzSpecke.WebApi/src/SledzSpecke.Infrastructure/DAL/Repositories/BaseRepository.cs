using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using System.Linq.Expressions;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal abstract class BaseRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly SledzSpeckeDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseRepository(SledzSpeckeDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        // For entities with value object IDs (like User with UserId), we need to use the value object
        // Check if the entity has an Id property that is a value object
        var entityType = Context.Model.FindEntityType(typeof(TEntity));
        var keyProperty = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault();
        
        if (keyProperty != null && keyProperty.ClrType != typeof(int))
        {
            // The key is a value object, create an instance
            var keyInstance = Activator.CreateInstance(keyProperty.ClrType, id);
            return await DbSet.FindAsync(new object[] { keyInstance! }, cancellationToken);
        }
        
        // Standard int key
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetBySpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(specification.ToExpression())
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetBySpecificationAsync<TKey>(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TKey>> orderBy,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(specification.ToExpression());
        
        query = ascending
            ? query.OrderBy(orderBy)
            : query.OrderByDescending(orderBy);
            
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> GetSingleBySpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(specification.ToExpression())
            .SingleOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await DbSet.CountAsync(cancellationToken)
            : await DbSet.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public virtual void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }

    protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        return DbSet.Where(specification.ToExpression());
    }

    protected async Task<List<TEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = ascending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<int> CountBySpecificationAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(specification.ToExpression())
            .CountAsync(cancellationToken);
    }

    public virtual async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        ISpecification<TEntity> specification,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(specification.ToExpression());

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = ascending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}