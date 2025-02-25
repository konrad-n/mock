using SQLite;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public interface IMigration
    {
        int Version { get; }
        string Description { get; }
        Task UpAsync(SQLiteAsyncConnection connection);
        Task DownAsync(SQLiteAsyncConnection connection);
    }
}