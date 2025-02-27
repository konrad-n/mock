using SledzSpecke.Infrastructure.Database.Context;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly Func<SQLiteAsyncConnection> _connectionFactory;
        private readonly Dictionary<int, IMigration> _migrations;

        public MigrationRunner(Func<SQLiteAsyncConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _migrations = new Dictionary<int, IMigration>
        {
            { 1, new M001_InitialSchema(_connectionFactory()) },
            // Add new migration
            { 19, new M019_EnhanceSpecializationModels(_connectionFactory()) }
        };
        }

        public async Task<int> GetCurrentVersionAsync()
        {
            var conn = _connectionFactory();
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
                await migration.Value.UpAsync(_connectionFactory());
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
                await migration.Value.DownAsync(_connectionFactory());
                await DeleteVersionAsync(migration.Key);
            }
        }

        private async Task UpdateVersionAsync(int version, string description)
        {
            var conn = _connectionFactory();
            try
            {
                var versionInfo = new VersionInfo
                {
                    Version = version,
                    AppliedOn = DateTime.UtcNow,
                    Description = description
                };
                await conn.InsertAsync(versionInfo);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE constraint failed"))
            {
                System.Diagnostics.Debug.WriteLine($"Version {version} already exists, updating information");
                var existingVersion = await conn.Table<VersionInfo>().Where(v => v.Version == version).FirstOrDefaultAsync();
                if (existingVersion != null)
                {
                    existingVersion.AppliedOn = DateTime.UtcNow;
                    existingVersion.Description = description;
                    await conn.UpdateAsync(existingVersion);
                }
            }
        }

        private async Task DeleteVersionAsync(int version)
        {
            var conn = _connectionFactory();
            await conn.DeleteAsync<VersionInfo>(version);
        }
    }
}
