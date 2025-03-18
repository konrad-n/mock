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
            try
            {
                System.Diagnostics.Debug.WriteLine("LogoutAsync: Rozpoczęto wylogowywanie użytkownika");

                // Zapamiętaj ID użytkownika przed wylogowaniem (do celów diagnostycznych)
                int userId = 0;
                if (this.currentUser != null)
                {
                    userId = this.currentUser.UserId;
                    System.Diagnostics.Debug.WriteLine($"LogoutAsync: Wylogowywanie użytkownika {this.currentUser.Username}, ID={userId}");
                }

                // Usuń ID użytkownika z ustawień
                await SettingsHelper.SetCurrentUserIdAsync(0);
                System.Diagnostics.Debug.WriteLine("LogoutAsync: Usunięto ID użytkownika z ustawień");

                // Wyczyszczenie pamięci podręcznej bieżącego użytkownika
                this.currentUser = null;
                System.Diagnostics.Debug.WriteLine("LogoutAsync: Wyczyszczono pamięć podręczną użytkownika");

                // Wyczyść inne ustawienia powiązane z użytkownikiem
                await SettingsHelper.SetCurrentModuleIdAsync(0);
                System.Diagnostics.Debug.WriteLine("LogoutAsync: Zresetowano ID aktualnego modułu");

                // Dodatkowe czyszczenie - można tu dodać inne operacje czyszczenia stanu aplikacji

                System.Diagnostics.Debug.WriteLine($"LogoutAsync: Użytkownik (ID={userId}) został pomyślnie wylogowany");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LogoutAsync: Błąd podczas wylogowywania - {ex.Message}");
                // Nawet w przypadku błędu, upewnij się że pamięć podręczna jest wyczyszczona
                this.currentUser = null;
            }
        }

        public async Task<bool> RegisterAsync(User user, string password, Models.Specialization specialization)
        {
            try
            {
                // Sprawdzenie czy użytkownik już istnieje
                var existingUser = await this.databaseService.GetUserByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    return false;
                }

                // WAŻNE: Najpierw zapisujemy specjalizację jako nowy rekord
                specialization.SpecializationId = 0; // Wymuszamy utworzenie nowego rekordu
                int specializationId = await this.databaseService.SaveSpecializationAsync(specialization);

                // Przypisujemy nowe ID specjalizacji do użytkownika
                user.SpecializationId = specializationId;
                user.PasswordHash = this.HashPassword(password);
                user.RegistrationDate = DateTime.Now;

                // Zapisujemy użytkownika
                int userId = await this.databaseService.SaveUserAsync(user);

                // Inicjalizujemy moduły dla tej konkretnej specjalizacji
                foreach (var module in specialization.Modules)
                {
                    module.ModuleId = 0; // Wymuszamy utworzenie nowego rekordu
                    module.SpecializationId = specializationId; // Przypisujemy do nowej specjalizacji
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