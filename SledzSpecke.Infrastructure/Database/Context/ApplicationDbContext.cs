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
                    await _migrationRunner.RunMigrationsAsync();
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public SQLiteAsyncConnection GetConnection()
        {
            return _database;
        }
    }
}
