using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public interface IMigrationRunner
    {
        Task RunMigrationsAsync();
        Task RollbackAsync(int version);
        Task<int> GetCurrentVersionAsync();
    }
}
