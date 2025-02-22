using SledzSpecke.Core.Models.Domain;
using SQLite;

namespace SledzSpecke.Infrastructure.Database.Configuration
{
    public static class EntityConfiguration
    {
        public static void ConfigureIndexes(SQLiteAsyncConnection connection)
        {
            connection.CreateIndexAsync<User>("IX_User_Email", u => u.Email, true);
            connection.CreateIndexAsync<User>("IX_User_PWZ", u => u.PWZ, true);
            // Dodaj inne indeksy
        }
    }
}
