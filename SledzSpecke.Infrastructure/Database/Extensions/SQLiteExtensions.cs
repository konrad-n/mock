using SQLite;
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
    }
}
