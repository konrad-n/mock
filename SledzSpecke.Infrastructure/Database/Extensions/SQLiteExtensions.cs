using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Extensions
{
    public static class SQLiteExtensions
    {
        public static async Task<bool> TableExistsAsync<T>(
            this SQLiteAsyncConnection connection) where T : new()
        {
            var tableInfo = await connection.GetTableInfoAsync(typeof(T).Name);
            return tableInfo.Any();
        }

        public static async Task<bool> TableExistsAsync(
            this SQLiteAsyncConnection connection, string tableName)
        {
            var tableInfo = await connection.GetTableInfoAsync(tableName);
            return tableInfo.Any();
        }

        public static async Task<bool> ColumnExistsAsync(
            this SQLiteAsyncConnection connection, string tableName, string columnName)
        {
            var tableInfo = await connection.QueryAsync<TableInfoResult>($"PRAGMA table_info({tableName})");
            return tableInfo.Any(column => column.Name.Equals(columnName, System.StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<bool> SafeAddColumnAsync(
            this SQLiteAsyncConnection connection, string tableName, string columnName, string columnType, string defaultValue = null)
        {
            if (await connection.ColumnExistsAsync(tableName, columnName))
            {
                return false; // Column already exists
            }

            string sql = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType}";
            if (defaultValue != null)
            {
                sql += $" DEFAULT {defaultValue}";
            }

            await connection.ExecuteAsync(sql);
            return true;
        }

        public static async Task<int> SafeDeleteAsync<T>(
            this SQLiteAsyncConnection connection, object primaryKey) where T : new()
        {
            try
            {
                return await connection.DeleteAsync<T>(primaryKey);
            }
            catch (SQLiteException)
            {
                return 0;
            }
        }

        private class TableInfoResult
        {
            public int Cid { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public int NotNull { get; set; }
            public string DefaultValue { get; set; }
            public int Pk { get; set; }
        }
    }
}
