using SledzSpecke.Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public class MigrationRunner : IMigrationRunner
    {
        private readonly IApplicationDbContext _context;
        private readonly Dictionary<int, BaseMigration> _migrations;

        public Task<int> GetCurrentVersionAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task RollbackAsync(int version)
        {
            throw new System.NotImplementedException();
        }

        public async Task RunMigrationsAsync()
        {
            var currentVersion = await GetCurrentVersionAsync();
            var pendingMigrations = _migrations
                .Where(m => m.Key > currentVersion)
                .OrderBy(m => m.Key);

            foreach (var migration in pendingMigrations)
            {
                await migration.Value.UpAsync();
                await UpdateVersionAsync(migration.Key);
            }
        }
    }
}
