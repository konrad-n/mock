using SQLite;
using System.IO;
namespace SledzSpecke.Infrastructure.Database.Configuration
{
    public static class DatabaseConfig
    {
        public static string GetDatabasePath(string appDataPath)
        {
            return Path.Combine(appDataPath, "sledzspecke.db3");
        }

        public static SQLiteConnectionString GetConnectionString(string appDataPath)
        {
            return new SQLiteConnectionString(
                GetDatabasePath(appDataPath),
                true,
                key: null
            );
        }
    }
}