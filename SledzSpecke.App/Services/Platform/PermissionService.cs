using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using SledzSpecke.App.Services.Platform;

namespace SledzSpecke.App.Services.Platform
{
    public class PermissionService : IPermissionService
    {
        // Sprawdzanie statusu uprawnień
        public async Task<PermissionStatus> CheckPermissionAsync<TPermission>()
            where TPermission : Permissions.BasePermission, new()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<TPermission>();
                return status;
            }
            catch (Exception)
            {
                return PermissionStatus.Unknown;
            }
        }

        // Żądanie pojedynczego uprawnienia
        public async Task<PermissionStatus> RequestPermissionAsync<TPermission>()
            where TPermission : Permissions.BasePermission, new()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<TPermission>();

                if (status == PermissionStatus.Unknown)
                {
                    // If initial check returns Unknown, try requesting anyway
                    // This might help in some cases where the initial check fails
                    status = await Permissions.RequestAsync<TPermission>();
                    return status;
                }

                if (status == PermissionStatus.Granted)
                    return status;

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // iOS nie pozwala na ponowne pytanie - przekieruj do ustawień
                    return status;
                }

                status = await Permissions.RequestAsync<TPermission>();
                return status;
            }
            catch (Exception)
            {
                return PermissionStatus.Unknown;
            }
        }

        // Żądanie grupy uprawnień
        public async Task<IDictionary<Type, PermissionStatus>> RequestPermissionsAsync(
            params Type[] permissionTypes)
        {
            var results = new Dictionary<Type, PermissionStatus>();

            foreach (var permissionType in permissionTypes)
            {
                var status = await RequestPermissionByTypeAsync(permissionType);
                results.Add(permissionType, status);
            }

            return results;
        }

        // Obsługa odmowy uprawnień
        public async Task<bool> HandleDeniedPermissionAsync<TPermission>(
            string rationaleMessage)
            where TPermission : Permissions.BasePermission, new()
        {
            var status = await CheckPermissionAsync<TPermission>();

            if (status != PermissionStatus.Denied)
                return true;

            // Pokaż wyjaśnienie dlaczego potrzebujemy uprawnienia
            var shouldShowRationale = Permissions.ShouldShowRationale<TPermission>();

            if (shouldShowRationale)
            {
                var window = Application.Current?.Windows.FirstOrDefault();
                if (window?.Page is not null)
                {
                    var answer = await window.Page.DisplayAlert(
                        "Uprawnienie wymagane",
                        rationaleMessage,
                        "Otwórz ustawienia",
                        "Anuluj");

                    if (answer)
                    {
                        AppInfo.ShowSettingsUI();
                    }
                }
            }

            return false;
        }

        private async Task<PermissionStatus> RequestPermissionByTypeAsync(Type permissionType)
        {
            var method = typeof(Permissions).GetMethod(
                nameof(Permissions.RequestAsync),
                BindingFlags.Public | BindingFlags.Static);

            if (method == null)
            {
                // Log or handle the error that the method was not found
                return PermissionStatus.Unknown;
            }

            var genericMethod = method.MakeGenericMethod(permissionType);

            if (genericMethod == null)
            {
                // Log or handle the error that the generic method could not be created
                return PermissionStatus.Unknown;
            }

            try
            {
                var result = genericMethod.Invoke(null, null);
                if (result is Task<PermissionStatus> task)
                {
                    return await task;
                }
                else
                {
                    // Log or handle the error that the result is not of expected type
                    return PermissionStatus.Unknown;
                }
            }
            catch
            {
                return PermissionStatus.Unknown;
            }
        }
    }
}
