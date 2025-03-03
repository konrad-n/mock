using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Settings
{
    public interface ISettingsService
    {
        Task<int> GetCurrentUserIdAsync();

        Task SetCurrentUserIdAsync(int userId);

        Task<NotificationSettings> GetNotificationSettingsAsync();

        Task SaveNotificationSettingsAsync(NotificationSettings settings);

        Task<DateTime?> GetLastBackupDateAsync();

        Task<bool> CreateBackupAsync();

        Task<bool> RestoreBackupAsync();

        Task<DateTime?> GetLastExportDateAsync();

        Task SaveLastExportDateAsync(DateTime date);

        Task<string> GetAppVersionAsync();

        Task<DateTime> GetLastUpdateDateAsync();

        Task<bool> CheckForUpdatesAsync();

        Task<bool> GetIsDarkModeAsync();

        Task SetIsDarkModeAsync(bool isDarkMode);

        Task ClearAllSettingsAsync();
    }
}