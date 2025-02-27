using SQLite;
using SledzSpecke.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using SledzSpecke.Infrastructure.Services;

namespace SledzSpecke.Infrastructure.Database
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private readonly ILogger<DatabaseService> _logger;
        private readonly IFileSystemService _fileSystemService;
        private bool _isInitialized = false;

        public DatabaseService(IFileSystemService fileSystemService, ILogger<DatabaseService> logger)
        {
            _fileSystemService = fileSystemService;
            _logger = logger;
        }

        public async Task InitAsync()
        {
            if (_isInitialized)
                return;

            var databasePath = Path.Combine(_fileSystemService.GetAppDataDirectory(), "SledzSpecke.db3");
            _database = new SQLiteAsyncConnection(databasePath);

            // Create tables for all models
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<SpecializationType>();
            await _database.CreateTableAsync<Specialization>();
            await _database.CreateTableAsync<Course>();
            await _database.CreateTableAsync<Internship>();
            await _database.CreateTableAsync<MedicalProcedure>();
            await _database.CreateTableAsync<ProcedureEntry>();
            await _database.CreateTableAsync<DutyShift>();
            await _database.CreateTableAsync<SelfEducation>();
            await _database.CreateTableAsync<UserSettings>();

            _isInitialized = true;
            _logger.LogInformation("Database initialized at {Path}", databasePath);
        }

        public async Task<List<T>> GetAllAsync<T>() where T : new()
        {
            await InitAsync();
            return await _database.Table<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class, new()
        {
            await InitAsync();
            return await _database.FindAsync<T>(id);
        }

        public async Task<int> SaveAsync<T>(T item) where T : new()
        {
            await InitAsync();
            return await _database.InsertOrReplaceAsync(item);
        }

        public async Task<int> DeleteAsync<T>(T item) where T : new()
        {
            await InitAsync();
            return await _database.DeleteAsync(item);
        }

        public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            await InitAsync();
            return await _database.QueryAsync<T>(query, args);
        }

        public async Task<int> ExecuteAsync(string query, params object[] args)
        {
            await InitAsync();
            return await _database.ExecuteAsync(query, args);
        }

        public async Task<List<MedicalProcedure>> GetProceduresForInternshipAsync(int internshipId)
        {
            await InitAsync();
            return await _database.Table<MedicalProcedure>()
                .Where(p => p.InternshipId == internshipId)
                .ToListAsync();
        }

        public async Task<List<ProcedureEntry>> GetEntriesForProcedureAsync(int procedureId)
        {
            await InitAsync();
            return await _database.Table<ProcedureEntry>()
                .Where(e => e.ProcedureId == procedureId)
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesForModuleAsync(ModuleType moduleType)
        {
            await InitAsync();
            return await _database.Table<Course>()
                .Where(c => c.Module == moduleType)
                .ToListAsync();
        }

        public async Task<List<Internship>> GetInternshipsForModuleAsync(ModuleType moduleType)
        {
            await InitAsync();
            return await _database.Table<Internship>()
                .Where(i => i.Module == moduleType)
                .ToListAsync();
        }

        public async Task<UserSettings> GetUserSettingsAsync()
        {
            await InitAsync();
            var settings = await _database.Table<UserSettings>().FirstOrDefaultAsync();
            return settings ?? new UserSettings();
        }

        public async Task SaveUserSettingsAsync(UserSettings settings)
        {
            await InitAsync();
            await _database.InsertOrReplaceAsync(settings);
        }

        public async Task<Specialization> GetCurrentSpecializationAsync()
        {
            await InitAsync();
            var userSettings = await GetUserSettingsAsync();

            if (userSettings.CurrentSpecializationId == 0)
                return null;

            return await _database.FindAsync<Specialization>(userSettings.CurrentSpecializationId);
        }

        public async Task DeleteAllDataAsync()
        {
            await InitAsync();
            await _database.DeleteAllAsync<Course>();
            await _database.DeleteAllAsync<Internship>();
            await _database.DeleteAllAsync<MedicalProcedure>();
            await _database.DeleteAllAsync<ProcedureEntry>();
            await _database.DeleteAllAsync<DutyShift>();
            await _database.DeleteAllAsync<SelfEducation>();
            await _database.DeleteAllAsync<Specialization>();
            await _database.DeleteAllAsync<User>();
            _logger.LogInformation("All data deleted from the database");
        }
    }
}
