using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;
using SQLite;
using System.Diagnostics;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection database;
        private bool isInitialized = false;
        private readonly Dictionary<int, Models.Specialization> _specializationCache = new();
        private readonly Dictionary<int, List<Module>> _moduleCache = new();
        private readonly IExceptionHandlerService _exceptionHandler;
        private readonly ILoggingService _loggingService;

        public DatabaseService(IExceptionHandlerService exceptionHandler, ILoggingService loggingService)
        {
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var databasePath = Constants.DatabasePath;
                var databaseDirectory = Path.GetDirectoryName(databasePath);

                if (!Directory.Exists(databaseDirectory))
                {
                    Directory.CreateDirectory(databaseDirectory);
                }

                this.database = new SQLiteAsyncConnection(databasePath, Constants.Flags);

                await this.database.CreateTableAsync<User>();
                await this.database.CreateTableAsync<Models.Specialization>();
                await this.database.CreateTableAsync<Module>();
                await this.database.CreateTableAsync<Internship>();
                await this.database.CreateTableAsync<MedicalShift>();
                await this.database.CreateTableAsync<Procedure>();
                await this.database.CreateTableAsync<Course>();
                await this.database.CreateTableAsync<SelfEducation>();
                await this.database.CreateTableAsync<Publication>();
                await this.database.CreateTableAsync<EducationalActivity>();
                await this.database.CreateTableAsync<Absence>();
                await this.database.CreateTableAsync<Models.Recognition>();
                await this.database.CreateTableAsync<SpecializationProgram>();
                await this.database.CreateTableAsync<RealizedMedicalShiftOldSMK>();
                await this.database.CreateTableAsync<RealizedMedicalShiftNewSMK>();
                await this.database.CreateTableAsync<RealizedProcedureNewSMK>();
                await this.database.CreateTableAsync<RealizedProcedureOldSMK>();
                await this.database.CreateTableAsync<RealizedInternshipNewSMK>();
                await this.database.CreateTableAsync<RealizedInternshipOldSMK>();

                this.isInitialized = true;
            }, null, "Nie udało się zainicjalizować bazy danych.", 3, 1000);
        }

        public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new InvalidInputException(
                        "Query cannot be null or empty",
                        "Zapytanie nie może być puste");
                }

                return await this.database.QueryAsync<T>(query, args);
            },
            new Dictionary<string, object> { { "Query", query } },
            "Nie udało się wykonać zapytania do bazy danych");
        }

        public async Task<int> ExecuteAsync(string query, params object[] args)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new InvalidInputException(
                        "Query cannot be null or empty",
                        "Zapytanie nie może być puste");
                }

                return await this.database.ExecuteAsync(query, args);
            },
            new Dictionary<string, object> { { "Query", query } },
            "Nie udało się wykonać polecenia w bazie danych");
        }

        public async Task<int> UpdateAsync<T>(T item)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (item == null)
                {
                    throw new InvalidInputException(
                        "Item cannot be null",
                        "Element nie może być pusty");
                }

                return await this.database.UpdateAsync(item);
            },
            new Dictionary<string, object> { { "ItemType", typeof(T).Name } },
            "Nie udało się zaktualizować danych w bazie");
        }

        public void ClearCache()
        {
            _specializationCache.Clear();
            _moduleCache.Clear();
            _loggingService.LogInformation("Cache wyczyszczony");
        }

        private class TableInfo
        {
            public string name { get; set; }
        }
    }
}