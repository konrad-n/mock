using SledzSpecke.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SledzSpecke.App.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        public Task<bool> CheckForUpdatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task ClearAllSettingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateBackupAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAppVersionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCurrentUserIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetIsDarkModeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DateTime?> GetLastBackupDateAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DateTime?> GetLastExportDateAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DateTime> GetLastUpdateDateAsync()
        {
            throw new NotImplementedException();
        }

        public Task<NotificationSettings> GetNotificationSettingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestoreBackupAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveLastExportDateAsync(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task SaveNotificationSettingsAsync(NotificationSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task SetCurrentUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task SetIsDarkModeAsync(bool isDarkMode)
        {
            throw new NotImplementedException();
        }
    }
}
