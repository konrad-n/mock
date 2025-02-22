using SledzSpecke.Infrastructure.Database.Context;
using SQLite;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public class MigrationManager
    {
        private readonly IApplicationDbContext _context;
        private readonly List<IMigration> _migrations;

        public MigrationManager(IApplicationDbContext context)
        {
            _context = context;
            _migrations = new List<IMigration>();
        }

        private async Task<int> GetDatabaseVersionAsync()
        {
            var connection = _context.GetConnection();
            try
            {
                await connection.CreateTableAsync<DatabaseVersion>();
                var version = await connection.Table<DatabaseVersion>()
                    .OrderByDescending(v => v.Version)
                    .FirstOrDefaultAsync();
                return version?.Version ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private List<IMigration> GetPendingMigrations(int currentVersion)
        {
            return _migrations
                .Where(m => m.Version > currentVersion)
                .OrderBy(m => m.Version)
                .ToList();
        }

        private async Task ApplyMigrationAsync(IMigration migration)
        {
            var connection = _context.GetConnection();
            await migration.UpAsync(connection);
            await UpdateVersionAsync(migration.Version);
        }

        private async Task UpdateVersionAsync(int version)
        {
            var connection = _context.GetConnection();
            await connection.InsertAsync(new DatabaseVersion
            {
                Version = version,
                AppliedAt = DateTime.UtcNow
            });
        }
    }

    public class DatabaseVersion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Version { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
