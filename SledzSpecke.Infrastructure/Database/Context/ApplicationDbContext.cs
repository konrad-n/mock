using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Migrations;
using SQLite;
using System;
using System.IO;
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
            // 1) Basic argument check
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                throw new ArgumentException("Database path cannot be null, empty, or whitespace.", nameof(databasePath));
            }

            // 2) Check if directory exists (if you expect the containing folder to be present)
            var directory = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Directory not found for the specified database path: {directory}");
            }

            // 3) Check if the file exists (if you do *not* want to create it automatically)
            //    If you prefer SQLite to create the file automatically, you can remove or comment this out.
            if (!File.Exists(databasePath))
            {
                throw new FileNotFoundException($"Unable to find the SQLite database at the specified path: {databasePath}");
            }

            // 4) Attempt to create the database connection, and catch any errors.
            try
            {
                _database = new SQLiteAsyncConnection(databasePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create SQLite connection with the provided path '{databasePath}'. " +
                    "Check that your path is valid and accessible.",
                    ex
                );
            }

            _migrationRunner = migrationRunner ?? throw new ArgumentNullException(nameof(migrationRunner));
        }

        public async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                // Create tables
                await _database.CreateTableAsync<User>();
                await _database.CreateTableAsync<Specialization>();
                await _database.CreateTableAsync<ProcedureDefinition>();
                await _database.CreateTableAsync<ProcedureExecution>();
                await _database.CreateTableAsync<ProcedureRequirement>();
                await _database.CreateTableAsync<Duty>();
                await _database.CreateTableAsync<Internship>();
                await _database.CreateTableAsync<InternshipDefinition>();
                await _database.CreateTableAsync<InternshipModule>();
                await _database.CreateTableAsync<Course>();
                await _database.CreateTableAsync<CourseDefinition>();
                await _database.CreateTableAsync<DutyRequirement>();
                await _database.CreateTableAsync<NotificationInfo>();

                await _migrationRunner.RunMigrationsAsync();
                _isInitialized = true;
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
