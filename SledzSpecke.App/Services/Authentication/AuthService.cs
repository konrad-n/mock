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

                // Weryfikacja obecnego hasła
                if (!this.VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return false;
                }

                // Hashowanie nowego hasła
                user.PasswordHash = this.HashPassword(newPassword);

                // Zapisanie zaktualizowanego użytkownika
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
                    throw new InvalidOperationException("No user is currently logged in.");
                }

                this.currentUser = await this.databaseService.GetUserAsync(userId);
                if (this.currentUser == null)
                {
                    throw new KeyNotFoundException($"User with ID {userId} not found.");
                }
                return this.currentUser;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving current user: {ex.Message}");
                throw;
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

                // Weryfikacja hasła
                if (!this.VerifyPassword(password, user.PasswordHash))
                {
                    return false;
                }

                // Zapisanie ID użytkownika w ustawieniach
                await SettingsHelper.SetCurrentUserIdAsync(user.UserId);

                // Aktualizacja pamięci podręcznej bieżącego użytkownika
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
            // Usunięcie ID użytkownika z ustawień
            await SettingsHelper.SetCurrentUserIdAsync(0);

            // Wyczyszczenie pamięci podręcznej bieżącego użytkownika
            this.currentUser = null;
        }

        public async Task<bool> RegisterAsync(User user, string password, Models.Specialization specialization)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Rozpoczynam rejestrację użytkownika...");
                System.Diagnostics.Debug.WriteLine($"Użytkownik: {user?.Username}, Specjalizacja: {specialization?.Name}");

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("Obiekt użytkownika jest null!");
                    return false;
                }

                if (specialization == null)
                {
                    System.Diagnostics.Debug.WriteLine("Obiekt specjalizacji jest null!");
                    return false;
                }

                if (string.IsNullOrEmpty(password))
                {
                    System.Diagnostics.Debug.WriteLine("Hasło jest puste!");
                    return false;
                }

                // Sprawdzenie, czy nazwa użytkownika już istnieje
                var existingUser = await this.databaseService.GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    System.Diagnostics.Debug.WriteLine("Użytkownik o podanej nazwie już istnieje!");
                    return false;
                }

                // Hashowanie hasła
                user.PasswordHash = this.HashPassword(password);
                System.Diagnostics.Debug.WriteLine("Hasło zostało zahashowane.");

                // Zapisanie użytkownika, aby uzyskać ID
                System.Diagnostics.Debug.WriteLine("Zapisuję użytkownika...");
                int userId = await this.databaseService.SaveUserAsync(user);
                if (userId <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Błąd podczas zapisywania użytkownika - nieprawidłowe ID!");
                    return false;
                }
                System.Diagnostics.Debug.WriteLine($"Użytkownik zapisany z ID: {userId}");

                // Zapisanie specjalizacji
                System.Diagnostics.Debug.WriteLine("Zapisuję specjalizację...");
                specialization.SpecializationId = 0; // Upewnij się, że ID jest 0 przed zapisem
                int specializationId = await this.databaseService.SaveSpecializationAsync(specialization);
                if (specializationId <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Błąd podczas zapisywania specjalizacji - nieprawidłowe ID!");
                    return false;
                }
                System.Diagnostics.Debug.WriteLine($"Specjalizacja zapisana z ID: {specializationId}");

                // Aktualizacja użytkownika o ID specjalizacji
                user.SpecializationId = specializationId;
                await this.databaseService.SaveUserAsync(user);
                System.Diagnostics.Debug.WriteLine("Użytkownik zaktualizowany o ID specjalizacji.");

                // Jeśli specjalizacja posiada moduły, zapisz je
                if (specialization.HasModules && specialization.Modules != null && specialization.Modules.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Zapisuję {specialization.Modules.Count} moduły...");

                    foreach (var module in specialization.Modules)
                    {
                        module.ModuleId = 0; // Upewnij się, że ID jest 0 przed zapisem
                        module.SpecializationId = specializationId;
                        int moduleId = await this.databaseService.SaveModuleAsync(module);
                        System.Diagnostics.Debug.WriteLine($"Moduł zapisany z ID: {moduleId}");
                    }

                    // Ustawienie aktualnego modułu (domyślnie pierwszy moduł)
                    if (specialization.Modules.Count > 0 && specialization.Modules[0].ModuleId > 0)
                    {
                        specialization.CurrentModuleId = specialization.Modules[0].ModuleId;
                        await this.databaseService.UpdateSpecializationAsync(specialization);
                        System.Diagnostics.Debug.WriteLine($"Ustawiono bieżący moduł na ID: {specialization.CurrentModuleId}");
                    }
                }

                System.Diagnostics.Debug.WriteLine("Rejestracja zakończona pomyślnie!");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas rejestracji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        // Metody pomocnicze do hashowania hasła
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