using SledzSpecke.Core.Models.Domain;
using SQLite;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public class M001_InitialSchema : BaseMigration
    {
        public M001_InitialSchema(SQLiteAsyncConnection connection) : base(connection)
        {
            Version = 1;
            Description = "Initial schema creation";
        }

        public override async Task UpAsync()
        {
            await _connection.CreateTableAsync<User>();
            await _connection.CreateTableAsync<Specialization>();
            // pozostałe tabele
        }

        public override async Task DownAsync()
        {
            await _connection.DropTableAsync<User>();
            await _connection.DropTableAsync<Specialization>();
            // pozostałe tabele
        }
    }
}
