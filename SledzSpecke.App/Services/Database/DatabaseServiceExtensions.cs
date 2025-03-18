using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SledzSpecke.App.Services.Database;
using SQLite;

namespace SledzSpecke.App.Services.Database
{
    public static class DatabaseServiceExtensions
    {
        public static async Task<List<T>> QueryAsync<T>(this IDatabaseService databaseService, string query, params object[] args)
            where T : new()
        {
            await databaseService.InitializeAsync();
            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.QueryAsync<T>(query, args);
        }

        public static async Task<int> ExecuteAsync(this IDatabaseService databaseService, string query, params object[] args)
        {
            await databaseService.InitializeAsync();
            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.ExecuteAsync(query, args);
        }

        public static async Task<int> InsertAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            await databaseService.InitializeAsync();
            await CreateTableIfNotExistsAsync<T>(databaseService);

            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.InsertAsync(obj);
        }

        public static async Task<int> UpdateAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            await databaseService.InitializeAsync();
            await CreateTableIfNotExistsAsync<T>(databaseService);

            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.UpdateAsync(obj);
        }

        public static async Task<int> DeleteAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            await databaseService.InitializeAsync();
            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            return await database.DeleteAsync(obj);
        }

        private static async Task CreateTableIfNotExistsAsync<T>(IDatabaseService databaseService)
            where T : new()
        {
            var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
            }

            var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
            if (database == null)
            {
                throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
            }

            await database.CreateTableAsync<T>();
        }
    }
}