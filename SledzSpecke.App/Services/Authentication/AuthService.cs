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

        public async Task<User> GetCurrentUserAsync()
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
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var user = await this.GetCurrentUserAsync();
            return user != null;
        }

        public async Task<bool> LoginAsync(string username, string password)
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

        public async Task LogoutAsync()
        {
            await SettingsHelper.SetCurrentUserIdAsync(0);
            this.currentUser = null;
            await SettingsHelper.SetCurrentModuleIdAsync(0);
        }

        public async Task<bool> RegisterAsync(User user, string password, Models.Specialization specialization)
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
            this.databaseService.SaveUserAsync(user);

            foreach (var module in specialization.Modules)
            {
                module.ModuleId = 0;
                module.SpecializationId = specializationId;
                await this.databaseService.SaveModuleAsync(module);
            }

            return true;
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