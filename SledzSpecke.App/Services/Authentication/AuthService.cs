using System.Security.Cryptography;
using System.Text;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IDatabaseService databaseService;
        private User currentUser;

        public AuthService(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                var user = await this.GetCurrentUserAsync();
                if (user == null)
                {
                    return false;
                }

                if (!this.VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = this.HashPassword(newPassword);

                await this.databaseService.SaveUserAsync(user);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zmiany hasła: {ex.Message}");
                return false;
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            if (this.currentUser != null)
            {
                return this.currentUser;
            }

            try
            {
                int userId = await SettingsHelper.GetCurrentUserIdAsync();
                if (userId <= 0)
                {
                    return null;
                }

                this.currentUser = await this.databaseService.GetUserAsync(userId);
                return this.currentUser;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania bieżącego użytkownika: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var user = await this.GetCurrentUserAsync();
            return user != null;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas logowania: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                int userId = 0;
                if (this.currentUser != null)
                {
                    userId = this.currentUser.UserId;
                }

                await SettingsHelper.SetCurrentUserIdAsync(0);
                this.currentUser = null;
                await SettingsHelper.SetCurrentModuleIdAsync(0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LogoutAsync: Błąd podczas wylogowywania - {ex.Message}");
                this.currentUser = null;
            }
        }

        public async Task<bool> RegisterAsync(User user, string password, Models.Specialization specialization)
        {
            try
            {
                var existingUser = await this.databaseService.GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    return false;
                }

                specialization.SpecializationId = 0;
                int specializationId = await this.databaseService.SaveSpecializationAsync(specialization);
                user.SpecializationId = specializationId;
                user.PasswordHash = this.HashPassword(password);
                user.RegistrationDate = DateTime.Now;
                int userId = await this.databaseService.SaveUserAsync(user);

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
                System.Diagnostics.Debug.WriteLine($"Błąd podczas rejestracji: {ex.Message}");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return this.HashPassword(password) == hash;
        }
    }
}