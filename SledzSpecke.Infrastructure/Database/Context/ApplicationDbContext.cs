using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Migrations;
using SQLite;
using System;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Context
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly IMigrationRunner _migrationRunner;
        private bool _isInitialized;

        public ApplicationDbContext(string databasePath, IMigrationRunner migrationRunner)
        {
            _database = new SQLiteAsyncConnection(databasePath);
            _migrationRunner = migrationRunner;
        }

        public async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                try
                {
                    await _database.CreateTableAsync<User>();
                    await _database.CreateTableAsync<Specialization>();

                    await _database.CreateTableAsync<ProcedureDefinition>();
                    await _database.CreateTableAsync<ProcedureExecution>();
                    await _database.CreateTableAsync<ProcedureRequirement>();
                    await _database.CreateTableAsync<Duty>();
                    await _database.CreateTableAsync<DutyRequirement>();
                    await _database.CreateTableAsync<Course>();
                    await _database.CreateTableAsync<CourseDefinition>();
                    await _database.CreateTableAsync<Internship>();
                    await _database.CreateTableAsync<InternshipDefinition>();
                    await _database.CreateTableAsync<InternshipModule>();
                    await _database.CreateTableAsync<NotificationInfo>();

                    await _migrationRunner.RunMigrationsAsync();
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }

        public SQLiteAsyncConnection GetConnection()
        {
            return _database;
        }

        public async Task EnsureDatabaseCreatedAsync()
        {
            if (!_isInitialized)
            {
                await InitializeTablesAsync();
                await UpdateDatabaseSchemaAsync();
                _isInitialized = true;
            }
        }

        private async Task InitializeTablesAsync()
        {
            // Create tables in the correct order
            var tableTypes = new[]
            {
                typeof(User),
                typeof(Specialization),
                typeof(ProcedureDefinition),
                // other types
            };

            foreach (var type in tableTypes)
            {
                await _database.CreateTableAsync(type);
            }
        }

        private async Task UpdateDatabaseSchemaAsync()
        {
            // Run database schema migrations
            await _migrationRunner.RunMigrationsAsync();
        }
    }
}
