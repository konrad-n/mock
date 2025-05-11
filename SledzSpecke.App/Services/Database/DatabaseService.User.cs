using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public partial class DatabaseService
    {
        public async Task<User> GetUserAsync(int id)
        {
            await this.InitializeAsync();

            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var user = await this.database.Table<User>().FirstOrDefaultAsync(u => u.UserId == id);
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        $"User with ID {id} not found",
                        $"Nie znaleziono użytkownika o ID {id}",
                        null,
                        new Dictionary<string, object> { { "UserId", id } });
                }
                return user;
            },
            new Dictionary<string, object> { { "UserId", id } },
            $"Nie udało się pobrać użytkownika o ID {id}",
            3, 800);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new InvalidInputException(
                        "Username cannot be null or empty",
                        "Nazwa użytkownika nie może być pusta");
                }

                return await this.database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
            },
            new Dictionary<string, object> { { "Username", username } },
            $"Nie udało się pobrać użytkownika o nazwie {username}");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<User>().ToListAsync();
            }, null, "Nie udało się pobrać listy użytkowników");
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (user == null)
                {
                    throw new InvalidInputException(
                        "User cannot be null",
                        "Użytkownik nie może być pusty");
                }

                if (user.UserId != 0)
                {
                    await this.database.UpdateAsync(user);
                    return user.UserId;
                }
                else
                {
                    await this.database.InsertAsync(user);
                    var lastId = await this.database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    user.UserId = lastId;
                    return lastId;
                }
            },
            new Dictionary<string, object> { { "User", user?.UserId } },
            "Nie udało się zapisać danych użytkownika");
        }
    }
}