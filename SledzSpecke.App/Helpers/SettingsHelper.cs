using SledzSpecke.App.Services.Storage;

namespace SledzSpecke.App.Helpers
{
    public static class SettingsHelper
    {
        private static ISecureStorageService secureStorageService;

        static SettingsHelper()
        {
            secureStorageService = new SecureStorageService();
        }

        public static void SetSecureStorageService(ISecureStorageService service)
        {
            secureStorageService = service ?? throw new ArgumentNullException(nameof(service));
        }

        public static async Task<int> GetCurrentUserIdAsync()
        {
            return await secureStorageService.GetAsync(Constants.CurrentUserIdKey) is string idStr &&
                   int.TryParse(idStr, out int id) ? id : 0;
        }

        public static async Task<string> GetCurrentUsernameAsync()
        {
            var user = await secureStorageService.GetAsync(Constants.CurrentUserIdKey);
            if (!string.IsNullOrEmpty(user) && int.TryParse(user, out int userId))
            {
                // Pobranie nazwy użytkownika na podstawie ID
                // To rozwiązanie zakłada, że przechowujemy tylko ID użytkownika, a jego nazwę pobieramy z bazy
                var dbService = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.Services.Database.IDatabaseService>();
                if (dbService != null)
                {
                    var userEntity = await dbService.GetUserAsync(userId);
                    return userEntity?.Username ?? string.Empty;
                }
            }
            return string.Empty;
        }

        public static string GetCurrentUsername()
        {
            return secureStorageService.GetAsync(Constants.CurrentUsernameKey).GetAwaiter().GetResult() ?? string.Empty;
        }

        public static async Task SetCurrentUsernameAsync(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                await secureStorageService.SetAsync(Constants.CurrentUsernameKey, username);
            }
            else
            {
                secureStorageService.Remove(Constants.CurrentUsernameKey);
            }
        }

        public static async Task SetCurrentUserIdAsync(int userId)
        {
            if (userId > 0)
            {
                await secureStorageService.SetAsync(Constants.CurrentUserIdKey, userId.ToString());
            }
            else
            {
                secureStorageService.Remove(Constants.CurrentUserIdKey);
            }
        }

        public static async Task<int> GetCurrentModuleIdAsync()
        {
            return await secureStorageService.GetAsync(Constants.CurrentModuleIdKey) is string idStr &&
                   int.TryParse(idStr, out int id) ? id : 0;
        }

        public static async Task SetCurrentModuleIdAsync(int moduleId)
        {
            if (moduleId > 0)
            {
                await secureStorageService.SetAsync(Constants.CurrentModuleIdKey, moduleId.ToString());
            }
            else
            {
                secureStorageService.Remove(Constants.CurrentModuleIdKey);
            }
        }
    }
}