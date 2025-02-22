using SQLite;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Configuration
{
    public static class EntityConfiguration
    {
        public static async Task ConfigureIndexesAsync(SQLiteAsyncConnection connection)
        {
            await connection.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS IX_User_Email ON User(Email)");
            await connection.ExecuteAsync(
                "CREATE INDEX IF NOT EXISTS IX_User_PWZ ON User(PWZ)");
        }
    }
}
