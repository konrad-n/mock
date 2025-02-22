using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public abstract class BaseRepository<T> where T : BaseEntity, new()
    {
        protected readonly IApplicationDbContext _context;
        protected readonly SQLiteAsyncConnection _connection;

        protected BaseRepository(IApplicationDbContext context)
        {
            _context = context;
            _connection = context.GetConnection();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            await _context.InitializeAsync();
            return await _connection.GetAsync<T>(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            await _context.InitializeAsync();
            return await _connection.Table<T>().ToListAsync();
        }

        public virtual async Task<int> AddAsync(T entity)
        {
            await _context.InitializeAsync();
            return await _connection.InsertAsync(entity);
        }

        public virtual async Task<int> UpdateAsync(T entity)
        {
            await _context.InitializeAsync();
            return await _connection.UpdateAsync(entity);
        }

        public virtual async Task<int> DeleteAsync(int id)
        {
            await _context.InitializeAsync();
            return await _connection.DeleteAsync<T>(id);
        }

        protected virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            await _context.InitializeAsync();
            return await _connection.Table<T>().Where(predicate).FirstOrDefaultAsync();
        }

        protected virtual async Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            await _context.InitializeAsync();
            return await _connection.Table<T>().Where(predicate).ToListAsync();
        }
    }
}
