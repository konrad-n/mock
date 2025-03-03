using SledzSpecke.App.Services.Storage;

namespace SledzSpecke.Tests.TestHelpers
{
    /// <summary>
    /// Helper for securely storing settings.
    /// </summary>
    public static class Settings
    {
        private static ISecureStorageService secureStorageService;

        // Initialize with a default implementation
        static Settings()
        {
            // This will be replaced with a proper implementation during app startup
            // or with a test implementation during testing
            secureStorageService = new SecureStorageService();
        }

        /// <summary>
        /// Sets the secure storage service to use.
        /// </summary>
        /// <param name="service">The secure storage service implementation.</param>
        public static void SetSecureStorageService(ISecureStorageService service)
        {
            secureStorageService = service ?? throw new ArgumentNullException(nameof(service));
        }

        // Current User
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

        // Notifications
        public static async Task<bool> GetNotificationsEnabledAsync()
        {
            return await secureStorageService.GetAsync(Constants.NotificationsEnabledKey) is string enabled &&
                   bool.TryParse(enabled, out bool result) && result;
        }

        public static async Task SetNotificationsEnabledAsync(bool enabled)
        {
            await secureStorageService.SetAsync(Constants.NotificationsEnabledKey, enabled.ToString());
        }

        // Backup
        public static async Task<DateTime?> GetLastBackupDateAsync()
        {
            if (await secureStorageService.GetAsync(Constants.LastBackupDateKey) is string dateStr
                && DateTime.TryParse(dateStr, out DateTime date))
            {
                return date;
            }

            return null;
        }

        public static async Task SetLastBackupDateAsync(DateTime date)
        {
            await secureStorageService.SetAsync(Constants.LastBackupDateKey, date.ToString("O"));
        }

        // Export
        public static async Task<DateTime?> GetLastExportDateAsync()
        {
            if (await secureStorageService.GetAsync(Constants.LastExportDateKey) is string dateStr
                && DateTime.TryParse(dateStr, out DateTime date))
            {
                return date;
            }

            return null;
        }

        public static async Task SetLastExportDateAsync(DateTime date)
        {
            await secureStorageService.SetAsync(Constants.LastExportDateKey, date.ToString("O"));
        }

        // Dark Mode
        public static async Task<bool> GetIsDarkModeAsync()
        {
            return await secureStorageService.GetAsync(Constants.IsDarkModeKey) is string darkMode &&
                   bool.TryParse(darkMode, out bool result) && result;
        }

        public static async Task SetIsDarkModeAsync(bool isDarkMode)
        {
            await secureStorageService.SetAsync(Constants.IsDarkModeKey, isDarkMode.ToString());
        }

        // Current Module
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

        public static async Task<int?> GetReminderDaysInAdvanceAsync()
        {
            if (await secureStorageService.GetAsync(Constants.ReminderDaysInAdvanceKey) is string reminderDays
                && int.TryParse(reminderDays, out int days))
            {
                return days;
            }

            return Constants.DefaultReminderDaysInAdvance;
        }

        public static async Task SetReminderDaysInAdvanceAsync(int days)
        {
            await secureStorageService.SetAsync(Constants.ReminderDaysInAdvanceKey, days.ToString());
        }

        // Clear All
        public static void ClearAll()
        {
            secureStorageService.RemoveAll();
        }
    }
}