using SQLite;
using SledzSpecke.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using SledzSpecke.Infrastructure.Services;
using System.Threading;
using System;

namespace SledzSpecke.Infrastructure.Database
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly ILogger<DatabaseService> _logger;
        private readonly IFileSystemService _fileSystemService;
        private bool _isInitialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public DatabaseService(IFileSystemService fileSystemService, ILogger<DatabaseService> logger)
        {
            _fileSystemService = fileSystemService;
            _logger = logger;
        }

        public async Task InitAsync()
        {
            // Użyj semafora, aby zapobiec równoczesnej inicjalizacji z wielu wątków
            await _initLock.WaitAsync();

            try
            {
                if (_isInitialized)
                    return;

                var databasePath = Path.Combine(_fileSystemService.GetAppDataDirectory(), "SledzSpecke.db3");
                _logger.LogInformation("Initializing database at {Path}", databasePath);

                // Sprawdź, czy katalog istnieje
                var directory = Path.GetDirectoryName(databasePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    _logger.LogInformation("Created database directory at {Path}", directory);
                }

                // Utwórz połączenie z bazą danych z dodatkowymi opcjami
                var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;
                _database = new SQLiteAsyncConnection(databasePath, flags);

                // Create tables for all models
                await _database.CreateTableAsync<User>().ConfigureAwait(false);
                await _database.CreateTableAsync<SpecializationType>().ConfigureAwait(false);
                await _database.CreateTableAsync<Specialization>().ConfigureAwait(false);
                await _database.CreateTableAsync<Course>().ConfigureAwait(false);
                await _database.CreateTableAsync<Internship>().ConfigureAwait(false);
                await _database.CreateTableAsync<MedicalProcedure>().ConfigureAwait(false);
                await _database.CreateTableAsync<ProcedureEntry>().ConfigureAwait(false);
                await _database.CreateTableAsync<DutyShift>().ConfigureAwait(false);
                await _database.CreateTableAsync<SelfEducation>().ConfigureAwait(false);
                await _database.CreateTableAsync<UserSettings>().ConfigureAwait(false);

                _isInitialized = true;
                _logger.LogInformation("Database initialized successfully at {Path}", databasePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
                throw;
            }
            finally
            {
                _initLock.Release();
            }
        }

        private async Task EnsureInitializedAsync()
        {
            if (!_isInitialized)
            {
                await InitAsync();
            }
        }

        public async Task<List<T>> GetAllAsync<T>() where T : new()
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.Table<T>().ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all items of type {Type}", typeof(T).Name);
                return new List<T>();
            }
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class, new()
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.FindAsync<T>(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting item of type {Type} with ID {Id}", typeof(T).Name, id);
                return null;
            }
        }

        public async Task<int> SaveAsync<T>(T item) where T : new()
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.InsertOrReplaceAsync(item).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving item of type {Type}", typeof(T).Name);
                throw;
            }
        }

        public async Task<int> DeleteAsync<T>(T item) where T : new()
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.DeleteAsync(item).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item of type {Type}", typeof(T).Name);
                throw;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.QueryAsync<T>(query, args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query {Query} for type {Type}", query, typeof(T).Name);
                return new List<T>();
            }
        }

        public async Task<int> ExecuteAsync(string query, params object[] args)
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.ExecuteAsync(query, args).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing non-query {Query}", query);
                throw;
            }
        }

        public async Task<List<MedicalProcedure>> GetProceduresForInternshipAsync(int internshipId)
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.Table<MedicalProcedure>()
                    .Where(p => p.InternshipId == internshipId)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedures for internship {InternshipId}", internshipId);
                return new List<MedicalProcedure>();
            }
        }

        public async Task<List<ProcedureEntry>> GetEntriesForProcedureAsync(int procedureId)
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.Table<ProcedureEntry>()
                    .Where(e => e.ProcedureId == procedureId)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entries for procedure {ProcedureId}", procedureId);
                return new List<ProcedureEntry>();
            }
        }

        public async Task<List<Course>> GetCoursesForModuleAsync(ModuleType moduleType)
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.Table<Course>()
                    .Where(c => c.Module == moduleType)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses for module {ModuleType}", moduleType);
                return new List<Course>();
            }
        }

        public async Task<List<Internship>> GetInternshipsForModuleAsync(ModuleType moduleType)
        {
            await EnsureInitializedAsync();
            try
            {
                return await _database.Table<Internship>()
                    .Where(i => i.Module == moduleType)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting internships for module {ModuleType}", moduleType);
                return new List<Internship>();
            }
        }

        public async Task<UserSettings> GetUserSettingsAsync()
        {
            await EnsureInitializedAsync();
            try
            {
                _logger.LogDebug("Getting user settings");
                var settings = await _database.Table<UserSettings>().FirstOrDefaultAsync().ConfigureAwait(false);
                _logger.LogDebug("User settings retrieved: {Settings}", settings != null ? "Found" : "Not found");
                return settings ?? new UserSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user settings");
                return new UserSettings();
            }
        }

        public async Task SaveUserSettingsAsync(UserSettings settings)
        {
            await EnsureInitializedAsync();
            try
            {
                await _database.InsertOrReplaceAsync(settings).ConfigureAwait(false);
                _logger.LogInformation("User settings saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving user settings");
                throw;
            }
        }

        public async Task<Specialization> GetCurrentSpecializationAsync()
        {
            await EnsureInitializedAsync();
            try
            {
                var userSettings = await GetUserSettingsAsync().ConfigureAwait(false);

                if (userSettings.CurrentSpecializationId == 0)
                {
                    _logger.LogInformation("No current specialization ID found in settings");
                    return null;
                }

                var specialization = await _database.FindAsync<Specialization>(userSettings.CurrentSpecializationId).ConfigureAwait(false);
                _logger.LogDebug("Current specialization retrieved: {Specialization}", specialization != null ? specialization.Name : "Not found");
                return specialization;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current specialization");
                return null;
            }
        }

        public async Task DeleteAllDataAsync()
        {
            await EnsureInitializedAsync();
            try
            {
                await _database.DeleteAllAsync<Course>().ConfigureAwait(false);
                await _database.DeleteAllAsync<Internship>().ConfigureAwait(false);
                await _database.DeleteAllAsync<MedicalProcedure>().ConfigureAwait(false);
                await _database.DeleteAllAsync<ProcedureEntry>().ConfigureAwait(false);
                await _database.DeleteAllAsync<DutyShift>().ConfigureAwait(false);
                await _database.DeleteAllAsync<SelfEducation>().ConfigureAwait(false);
                await _database.DeleteAllAsync<Specialization>().ConfigureAwait(false);
                await _database.DeleteAllAsync<User>().ConfigureAwait(false);
                await _database.DeleteAllAsync<UserSettings>().ConfigureAwait(false);
                _logger.LogInformation("All data deleted from the database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all data");
                throw;
            }
        }
    }
}