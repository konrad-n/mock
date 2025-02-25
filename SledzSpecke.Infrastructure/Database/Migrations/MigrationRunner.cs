using SledzSpecke.Infrastructure.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly IApplicationDbContext _context;
        private readonly Dictionary<int, IMigration> _migrations;

        public MigrationRunner(IApplicationDbContext context)
        {
            _context = context;
            _migrations = new Dictionary<int, IMigration>
            {
                { 1, new M001_InitialSchema(context.GetConnection()) },
                // Add new migration
                { 19, new M019_EnhanceSpecializationModels(context.GetConnection()) }
            };
        }

        public async Task<int> GetCurrentVersionAsync()
        {
            var conn = _context.GetConnection();
            await conn.CreateTableAsync<VersionInfo>();
            var lastVersion = await conn.Table<VersionInfo>()
                .OrderByDescending(v => v.Version)
                .FirstOrDefaultAsync();
            return lastVersion?.Version ?? 0;
        }

        public async Task RunMigrationsAsync()
        {
            var currentVersion = await GetCurrentVersionAsync();
            var pendingMigrations = _migrations
                .Where(m => m.Key > currentVersion)
                .OrderBy(m => m.Key);

            foreach (var migration in pendingMigrations)
            {
                await migration.Value.UpAsync(_context.GetConnection());
                await UpdateVersionAsync(migration.Key, migration.Value.Description);
            }
        }

        public async Task RollbackAsync(int version)
        {
            var currentVersion = await GetCurrentVersionAsync();
            var migrationsToRollback = _migrations
                .Where(m => m.Key > version && m.Key <= currentVersion)
                .OrderByDescending(m => m.Key);

            foreach (var migration in migrationsToRollback)
            {
                await migration.Value.DownAsync(_context.GetConnection());
                await DeleteVersionAsync(migration.Key);
            }
        }

        private async Task UpdateVersionAsync(int version, string description)
        {
            var conn = _context.GetConnection();
            var versionInfo = new VersionInfo
            {
                Version = version,
                AppliedOn = DateTime.UtcNow,
                Description = description
            };
            await conn.InsertAsync(versionInfo);
        }

        private async Task DeleteVersionAsync(int version)
        {
            var conn = _context.GetConnection();
            await conn.DeleteAsync<VersionInfo>(version);
        }
    }
}
