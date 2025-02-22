using SQLite;
using System.IO;

namespace SledzSpecke.Infrastructure.Database.Configuration
{
    public static class DatabaseConfig
    {
        public static string GetDatabasePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, "sledzspecke.db3");
        }

        public static SQLiteConnectionString GetConnectionString()
        {
            return new SQLiteConnectionString(GetDatabasePath(),
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.SharedCache);
        }
    }
}
