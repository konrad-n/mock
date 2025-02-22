using SQLite;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Context
{
    public interface IApplicationDbContext
    {
        Task InitializeAsync();
        SQLiteAsyncConnection GetConnection();
    }
}
