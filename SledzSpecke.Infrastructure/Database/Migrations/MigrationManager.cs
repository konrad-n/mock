using SledzSpecke.Infrastructure.Database.Context;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public class MigrationManager
    {
        private readonly IApplicationDbContext _context;

        public async Task ApplyMigrationsAsync()
        {
            var currentVersion = await GetDatabaseVersionAsync();
            var migrations = GetPendingMigrations(currentVersion);

            foreach (var migration in migrations)
            {
                await ApplyMigrationAsync(migration);
            }
        }
    }
}
