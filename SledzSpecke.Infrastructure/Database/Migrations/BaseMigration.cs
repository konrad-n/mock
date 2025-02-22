using SQLite;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Migrations
{
    public abstract class BaseMigration
    {
        public int Version { get; protected set; }
        public string Description { get; protected set; }
        protected readonly SQLiteAsyncConnection _connection;

        protected BaseMigration(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public abstract Task UpAsync();
        public abstract Task DownAsync();
    }
}
