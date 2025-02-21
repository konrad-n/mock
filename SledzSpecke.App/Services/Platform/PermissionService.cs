using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.App.Services.Platform
{
    public class PermissionService
    {
        // Sprawdzanie statusu uprawnień
        public async Task<PermissionStatus> CheckPermissionStatus<TPermission>() where TPermission : Permissions.BasePermission, new()
        {
            return await Permissions.CheckStatusAsync<TPermission>();
        }

        // Żądanie pojedynczego uprawnienia
        public async Task<PermissionStatus> RequestPermission<TPermission>() where TPermission : Permissions.BasePermission, new()
        {
            try
            {
                var status = await CheckPermissionStatus<TPermission>();

                if (status == PermissionStatus.Granted)
                    return status;

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // Na iOS, jeśli użytkownik wcześniej odmówił, kierujemy do ustawień
                    await OpenSettings();
                    return status;
                }

                status = await Permissions.RequestAsync<TPermission>();
                return status;
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine($"Error requesting permission: {ex.Message}");
                return PermissionStatus.Unknown;
            }
        }

        // Żądanie grupy uprawnień
        public async Task<Dictionary<Type, PermissionStatus>> RequestPermissions(params Type[] permissionTypes)
        {
            var results = new Dictionary<Type, PermissionStatus>();

            foreach (var permissionType in permissionTypes)
            {
                if (typeof(Permissions.BasePermission).IsAssignableFrom(permissionType))
                {
                    var permission = (Permissions.BasePermission)Activator.CreateInstance(permissionType);
                    var status = await Permissions.RequestAsync(permission);
                    results.Add(permissionType, status);
                }
            }

            return results;
        }

        // Obsługa odmowy uprawnień
        public async Task HandlePermissionDenial<TPermission>(string permissionName) where TPermission : Permissions.BasePermission, new()
        {
            var shouldShowRationale = await Permissions.ShouldShowRationale<TPermission>();

            if (shouldShowRationale)
            {
                // Pokazujemy wyjaśnienie, dlaczego potrzebujemy uprawnienia
                var result = await Application.Current.MainPage.DisplayAlert(
                    "Uprawnienie wymagane",
                    $"Aplikacja potrzebuje uprawnienia {permissionName} do prawidłowego działania.",
                    "Otwórz ustawienia",
                    "Anuluj");

                if (result)
                {
                    await OpenSettings();
                }
            }
            else
            {
                // Użytkownik zaznaczył "Nie pytaj ponownie"
                await Application.Current.MainPage.DisplayAlert(
                    "Uprawnienie niedostępne",
                    $"Uprawnienie {permissionName} jest wymagane. Proszę włącz je w ustawieniach aplikacji.",
                    "OK");
            }
        }

        // Pomocnicza metoda do otwierania ustawień
        private async Task OpenSettings()
        {
            try
            {
                await AppInfo.ShowSettingsUI();
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine($"Error opening settings: {ex.Message}");
            }
        }
    }
}
