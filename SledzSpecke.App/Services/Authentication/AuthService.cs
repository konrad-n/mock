using System.Security.Cryptography;
using System.Text;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;

namespace SledzSpecke.App.Services.Authentication
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IDatabaseService databaseService;
        private User currentUser;

        public AuthService(
            IDatabaseService databaseService,
            IExceptionHandlerService exceptionHandler,
            ILoggingService logger) : base(exceptionHandler, logger)
        {
            this.databaseService = databaseService;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                if (this.currentUser != null)
                {
                    return this.currentUser;
                }

                int userId = await SettingsHelper.GetCurrentUserIdAsync();
                if (userId <= 0)
                {
                    return null;
                }

                this.currentUser = await this.databaseService.GetUserAsync(userId);
                return this.currentUser;
            }, "Nie udało się pobrać danych użytkownika.");
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.GetCurrentUserAsync();
                return user != null;
            }, "Nie udało się sprawdzić stanu uwierzytelnienia.");
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.databaseService.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return false;
                }

                if (!this.VerifyPassword(password, user.PasswordHash))
                {
                    return false;
                }

                await SettingsHelper.SetCurrentUserIdAsync(user.UserId);
                this.currentUser = user;

                return true;
            }, "Nie udało się zalogować użytkownika.");
        }

        public async Task LogoutAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await SettingsHelper.SetCurrentUserIdAsync(0);
                this.currentUser = null;
                await SettingsHelper.SetCurrentModuleIdAsync(0);
            }, "Nie udało się wylogować użytkownika.");
        }

        public async Task<bool> RegisterAsync(User user, string password, Models.Specialization specialization)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (user == null)
                {
                    throw new InvalidInputException("User cannot be null", "Użytkownik nie może być pusty.");
                }

                var existingUser = await this.databaseService.GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    return false;
                }

                try
                {
                    specialization.SpecializationId = 0;
                    int specializationId = await this.databaseService.SaveSpecializationAsync(specialization);
                    user.SpecializationId = specializationId;
                    user.PasswordHash = HashPassword(password);
                    user.RegistrationDate = DateTime.Now;
                    await this.databaseService.SaveUserAsync(user);

                    foreach (var module in specialization.Modules)
                    {
                        module.ModuleId = 0;
                        module.SpecializationId = specializationId;
                        await this.databaseService.SaveModuleAsync(module);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Błąd podczas rejestracji użytkownika",
                        new Dictionary<string, object> { { "Username", user.Username } });
                    throw;
                }
            }, "Nie udało się zarejestrować użytkownika.");
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
