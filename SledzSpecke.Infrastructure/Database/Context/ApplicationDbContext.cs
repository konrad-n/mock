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
                    System.Diagnostics.Debug.WriteLine("Creating User table...");
                    await _database.CreateTableAsync<User>();

                    System.Diagnostics.Debug.WriteLine("Creating Specialization table...");
                    await _database.CreateTableAsync<Specialization>();

                    System.Diagnostics.Debug.WriteLine("Creating ProcedureDefinition table...");
                    await _database.CreateTableAsync<ProcedureDefinition>();

                    System.Diagnostics.Debug.WriteLine("Creating ProcedureExecution table...");
                    await _database.CreateTableAsync<ProcedureExecution>();

                    System.Diagnostics.Debug.WriteLine("Creating ProcedureRequirement table...");
                    await _database.CreateTableAsync<ProcedureRequirement>();

                    System.Diagnostics.Debug.WriteLine("Creating Duty table...");
                    await _database.CreateTableAsync<Duty>();

                    System.Diagnostics.Debug.WriteLine("Creating DutyRequirement table...");
                    await _database.CreateTableAsync<DutyRequirement>();

                    System.Diagnostics.Debug.WriteLine("Creating Course table...");
                    await _database.CreateTableAsync<Course>();

                    System.Diagnostics.Debug.WriteLine("Creating CourseDefinition table...");
                    await _database.CreateTableAsync<CourseDefinition>();

                    System.Diagnostics.Debug.WriteLine("Creating Internship table...");
                    await _database.CreateTableAsync<Internship>();

                    System.Diagnostics.Debug.WriteLine("Creating InternshipDefinition table...");
                    await _database.CreateTableAsync<InternshipDefinition>();

                    System.Diagnostics.Debug.WriteLine("Creating InternshipModule table...");
                    await _database.CreateTableAsync<InternshipModule>();

                    System.Diagnostics.Debug.WriteLine("Creating NotificationInfo table...");
                    await _database.CreateTableAsync<NotificationInfo>();

                    await _migrationRunner.RunMigrationsAsync();
                    _isInitialized = true;
                    System.Diagnostics.Debug.WriteLine("Database initialization completed successfully");
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
