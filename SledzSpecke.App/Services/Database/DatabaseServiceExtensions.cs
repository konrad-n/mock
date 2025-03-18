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
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wykonywania zapytania SQL: {ex.Message}");
                return new List<T>();
            }
        }

        public static async Task<int> ExecuteAsync(this IDatabaseService databaseService, string query, params object[] args)
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wykonywania zapytania SQL: {ex.Message}");
                return 0;
            }
        }

        public static async Task<int> InsertAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wstawiania obiektu: {ex.Message}");
                return 0;
            }
        }

        public static async Task<int> UpdateAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas aktualizacji obiektu: {ex.Message}");
                return 0;
            }
        }

        public static async Task<int> DeleteAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania obiektu: {ex.Message}");
                return 0;
            }
        }

        private static async Task CreateTableIfNotExistsAsync<T>(IDatabaseService databaseService)
            where T : new()
        {
            try
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

                // Utwórz tabelę
                await database.CreateTableAsync<T>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas tworzenia tabeli: {ex.Message}");
            }
        }
    }
}