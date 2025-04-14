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