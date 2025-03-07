using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SledzSpecke.App.Services.Database;
using SQLite;

namespace SledzSpecke.App.Services.Database
{
    /// <summary>
    /// Klasa rozszerzająca IDatabaseService o metody SQL.
    /// </summary>
    public static class DatabaseServiceExtensions
    {
        /// <summary>
        /// Wykonuje zapytanie SQL i zwraca listę obiektów danego typu.
        /// </summary>
        /// <typeparam name="T">Typ obiektu wynikowego.</typeparam>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <param name="query">Zapytanie SQL.</param>
        /// <param name="args">Argumenty zapytania.</param>
        /// <returns>Lista obiektów danego typu.</returns>
        public static async Task<List<T>> QueryAsync<T>(this IDatabaseService databaseService, string query, params object[] args)
            where T : new()
        {
            try
            {
                // Inicjalizacja bazy danych
                await databaseService.InitializeAsync();

                // Reflection, aby uzyskać dostęp do prywatnego pola database
                var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field == null)
                {
                    throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
                }

                // Pobierz obiekt SQLiteAsyncConnection
                var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
                if (database == null)
                {
                    throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
                }

                // Wykonaj zapytanie
                return await database.QueryAsync<T>(query, args);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wykonywania zapytania SQL: {ex.Message}");
                return new List<T>();
            }
        }

        /// <summary>
        /// Wykonuje zapytanie SQL, które nie zwraca wyników.
        /// </summary>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <param name="query">Zapytanie SQL.</param>
        /// <param name="args">Argumenty zapytania.</param>
        /// <returns>Liczba zmodyfikowanych wierszy.</returns>
        public static async Task<int> ExecuteAsync(this IDatabaseService databaseService, string query, params object[] args)
        {
            try
            {
                // Inicjalizacja bazy danych
                await databaseService.InitializeAsync();

                // Reflection, aby uzyskać dostęp do prywatnego pola database
                var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field == null)
                {
                    throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
                }

                // Pobierz obiekt SQLiteAsyncConnection
                var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
                if (database == null)
                {
                    throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
                }

                // Wykonaj zapytanie
                return await database.ExecuteAsync(query, args);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wykonywania zapytania SQL: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Wstawia obiekt do bazy danych.
        /// </summary>
        /// <typeparam name="T">Typ obiektu.</typeparam>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <param name="obj">Obiekt do wstawienia.</param>
        /// <returns>Identyfikator wstawionego obiektu.</returns>
        public static async Task<int> InsertAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            try
            {
                // Inicjalizacja bazy danych
                await databaseService.InitializeAsync();

                // Utwórz tabelę, jeśli nie istnieje
                await CreateTableIfNotExistsAsync<T>(databaseService);

                // Reflection, aby uzyskać dostęp do prywatnego pola database
                var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field == null)
                {
                    throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
                }

                // Pobierz obiekt SQLiteAsyncConnection
                var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
                if (database == null)
                {
                    throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
                }

                // Wstaw obiekt
                return await database.InsertAsync(obj);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas wstawiania obiektu: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Aktualizuje obiekt w bazie danych.
        /// </summary>
        /// <typeparam name="T">Typ obiektu.</typeparam>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <param name="obj">Obiekt do aktualizacji.</param>
        /// <returns>Liczba zaktualizowanych wierszy.</returns>
        public static async Task<int> UpdateAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            try
            {
                // Inicjalizacja bazy danych
                await databaseService.InitializeAsync();

                // Utwórz tabelę, jeśli nie istnieje
                await CreateTableIfNotExistsAsync<T>(databaseService);

                // Reflection, aby uzyskać dostęp do prywatnego pola database
                var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field == null)
                {
                    throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
                }

                // Pobierz obiekt SQLiteAsyncConnection
                var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
                if (database == null)
                {
                    throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
                }

                // Aktualizuj obiekt
                return await database.UpdateAsync(obj);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas aktualizacji obiektu: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Usuwa obiekt z bazy danych.
        /// </summary>
        /// <typeparam name="T">Typ obiektu.</typeparam>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <param name="obj">Obiekt do usunięcia.</param>
        /// <returns>Liczba usuniętych wierszy.</returns>
        public static async Task<int> DeleteAsync<T>(this IDatabaseService databaseService, T obj)
            where T : new()
        {
            try
            {
                // Inicjalizacja bazy danych
                await databaseService.InitializeAsync();

                // Reflection, aby uzyskać dostęp do prywatnego pola database
                var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field == null)
                {
                    throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
                }

                // Pobierz obiekt SQLiteAsyncConnection
                var database = field.GetValue(databaseService) as SQLiteAsyncConnection;
                if (database == null)
                {
                    throw new InvalidOperationException("Pole 'database' nie jest typu SQLiteAsyncConnection.");
                }

                // Usuń obiekt
                return await database.DeleteAsync(obj);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania obiektu: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Tworzy tabelę w bazie danych, jeśli nie istnieje.
        /// </summary>
        /// <typeparam name="T">Typ obiektu.</typeparam>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <returns>Task.</returns>
        private static async Task CreateTableIfNotExistsAsync<T>(IDatabaseService databaseService)
            where T : new()
        {
            try
            {
                // Reflection, aby uzyskać dostęp do prywatnego pola database
                var field = typeof(DatabaseService).GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field == null)
                {
                    throw new InvalidOperationException("Nie znaleziono pola 'database' w klasie DatabaseService.");
                }

                // Pobierz obiekt SQLiteAsyncConnection
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