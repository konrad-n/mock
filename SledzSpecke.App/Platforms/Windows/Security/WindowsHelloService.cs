using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Windows.Security.Credentials;
using Windows.Storage;

namespace SledzSpecke.App.Platforms.Windows.Security
{
    public class WindowsHelloService : IWindowsSecurityService
    {
        private readonly ILogger<WindowsHelloService> _logger;
        private const string KEYNAME = "SledzSpeckeAuth";
        
        public WindowsHelloService(ILogger<WindowsHelloService> logger)
        {
            _logger = logger;
        }
        
        public async Task<bool> IsWindowsHelloAvailableAsync()
        {
            try
            {
                var keyCredentialAvailability = await KeyCredentialManager.IsSupportedAsync();
                return keyCredentialAvailability;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Windows Hello availability");
                return false;
            }
        }
        
        public async Task<(bool Success, string Message)> AuthenticateWithWindowsHelloAsync(string username)
        {
            try
            {
                // Sprawdź, czy Windows Hello jest dostępne
                if (!await IsWindowsHelloAvailableAsync())
                {
                    return (false, "Windows Hello nie jest dostępne na tym urządzeniu.");
                }
                
                // Sprawdź, czy istnieje już konto dla tego użytkownika
                var consentResult = await KeyCredentialManager.RequestCreateAsync(
                    KEYNAME + username, 
                    KeyCredentialCreationOption.ReplaceExisting);
                
                if (consentResult.Status == KeyCredentialStatus.Success)
                {
                    // Zapisz informację o pomyślnym uwierzytelnieniu
                    ApplicationData.Current.LocalSettings.Values["WindowsHelloAuthenticated"] = true;
                    ApplicationData.Current.LocalSettings.Values["WindowsHelloUsername"] = username;
                    
                    return (true, "Pomyślnie uwierzytelniono za pomocą Windows Hello.");
                }
                else if (consentResult.Status == KeyCredentialStatus.UserCanceled)
                {
                    return (false, "Uwierzytelnianie zostało anulowane przez użytkownika.");
                }
                else if (consentResult.Status == KeyCredentialStatus.NotFound)
                {
                    return (false, "Nie znaleziono klucza uwierzytelniającego. Proszę zarejestrować się ponownie.");
                }
                else
                {
                    return (false, $"Błąd podczas uwierzytelniania: {consentResult.Status}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating with Windows Hello");
                return (false, $"Wystąpił błąd podczas uwierzytelniania: {ex.Message}");
            }
        }
        
        public async Task<bool> RegisterUserWithWindowsHelloAsync(string username)
        {
            try
            {
                // Sprawdź, czy Windows Hello jest dostępne
                if (!await IsWindowsHelloAvailableAsync())
                {
                    return false;
                }
                
                // Spróbuj utworzyć lub zastąpić istniejące poświadczenia
                var consentResult = await KeyCredentialManager.RequestCreateAsync(
                    KEYNAME + username, 
                    KeyCredentialCreationOption.ReplaceExisting);
                
                if (consentResult.Status == KeyCredentialStatus.Success)
                {
                    // Zapisz informację o pomyślnej rejestracji
                    ApplicationData.Current.LocalSettings.Values["WindowsHelloRegistered"] = true;
                    ApplicationData.Current.LocalSettings.Values["WindowsHelloUsername"] = username;
                    
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with Windows Hello");
                return false;
            }
        }
        
        public async Task<bool> RemoveWindowsHelloCredentialsAsync(string username)
        {
            try
            {
                await KeyCredentialManager.DeleteAsync(KEYNAME + username);
                
                // Usuń informacje o rejestracji
                ApplicationData.Current.LocalSettings.Values.Remove("WindowsHelloRegistered");
                ApplicationData.Current.LocalSettings.Values.Remove("WindowsHelloUsername");
                ApplicationData.Current.LocalSettings.Values.Remove("WindowsHelloAuthenticated");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing Windows Hello credentials");
                return false;
            }
        }
    }
    
    public interface IWindowsSecurityService
    {
        Task<bool> IsWindowsHelloAvailableAsync();
        Task<(bool Success, string Message)> AuthenticateWithWindowsHelloAsync(string username);
        Task<bool> RegisterUserWithWindowsHelloAsync(string username);
        Task<bool> RemoveWindowsHelloCredentialsAsync(string username);
    }
}
