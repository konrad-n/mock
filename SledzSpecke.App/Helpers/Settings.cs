namespace SledzSpecke.App.Helpers
{
    public static class Settings
    {
        // Current User
        public static async Task<int> GetCurrentUserIdAsync()
        {
            return await SecureStorage.GetAsync(Constants.CurrentUserIdKey) is string idStr &&
                   int.TryParse(idStr, out int id) ? id : 0;
        }

        public static async Task SetCurrentUserIdAsync(int userId)
        {
            if (userId > 0)
            {
                await SecureStorage.SetAsync(Constants.CurrentUserIdKey, userId.ToString());
            }
            else
            {
                SecureStorage.Remove(Constants.CurrentUserIdKey);
            }
        }

        // Notifications
        public static async Task<bool> GetNotificationsEnabledAsync()
        {
            return await SecureStorage.GetAsync(Constants.NotificationsEnabledKey) is string enabled &&
                   bool.TryParse(enabled, out bool result) && result;
        }

        public static async Task SetNotificationsEnabledAsync(bool enabled)
        {
            await SecureStorage.SetAsync(Constants.NotificationsEnabledKey, enabled.ToString());
        }

        // Backup
        public static async Task<DateTime?> GetLastBackupDateAsync()
        {
            if (await SecureStorage.GetAsync(Constants.LastBackupDateKey) is string dateStr
                && DateTime.TryParse(dateStr, out DateTime date))
            {
                return date;
            }

            return null;
        }

        public static async Task SetLastBackupDateAsync(DateTime date)
        {
            await SecureStorage.SetAsync(Constants.LastBackupDateKey, date.ToString("O"));
        }

        // Export
        public static async Task<DateTime?> GetLastExportDateAsync()
        {
            if (await SecureStorage.GetAsync(Constants.LastExportDateKey) is string dateStr
                && DateTime.TryParse(dateStr, out DateTime date))
            {
                return date;
            }

            return null;
        }

        public static async Task SetLastExportDateAsync(DateTime date)
        {
            await SecureStorage.SetAsync(Constants.LastExportDateKey, date.ToString("O"));
        }

        // Dark Mode
        public static async Task<bool> GetIsDarkModeAsync()
        {
            return await SecureStorage.GetAsync(Constants.IsDarkModeKey) is string darkMode &&
                   bool.TryParse(darkMode, out bool result) && result;
        }

        public static async Task SetIsDarkModeAsync(bool isDarkMode)
        {
            await SecureStorage.SetAsync(Constants.IsDarkModeKey, isDarkMode.ToString());
        }

        // Current Module
        public static async Task<int> GetCurrentModuleIdAsync()
        {
            return await SecureStorage.GetAsync(Constants.CurrentModuleIdKey) is string idStr &&
                   int.TryParse(idStr, out int id) ? id : 0;
        }

        public static async Task SetCurrentModuleIdAsync(int moduleId)
        {
            if (moduleId > 0)
            {
                await SecureStorage.SetAsync(Constants.CurrentModuleIdKey, moduleId.ToString());
            }
            else
            {
                SecureStorage.Remove(Constants.CurrentModuleIdKey);
            }
        }

        public static async Task<int?> GetReminderDaysInAdvanceAsync()
        {
            if (await SecureStorage.GetAsync(Constants.ReminderDaysInAdvanceKey) is string reminderDays
                && int.TryParse(reminderDays, out int days))
            {
                return days;
            }

            return Constants.DefaultReminderDaysInAdvance;
        }

        public static async Task SetReminderDaysInAdvanceAsync(int days)
        {
            await SecureStorage.SetAsync(Constants.ReminderDaysInAdvanceKey, days.ToString());
        }

        // Clear All
        public static void ClearAll()
        {
            SecureStorage.RemoveAll();
        }
    }
}