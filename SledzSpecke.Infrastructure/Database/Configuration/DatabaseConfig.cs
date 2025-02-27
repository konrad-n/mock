using SQLite;
using System;
using System.IO;

namespace SledzSpecke.Infrastructure.Database.Configuration
{
    public static class DatabaseConfig
    {
        public static string GetDatabasePath(string appDataPath)
        {
            var databaseName = $"sledzspecke_{DateTime.Now.Ticks}.db3";
            return Path.Combine(appDataPath, databaseName);
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
