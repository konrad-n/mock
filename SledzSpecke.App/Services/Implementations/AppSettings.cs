using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class AppSettings : IAppSettings
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<AppSettings> _logger;
        private UserSettings _settings;

        public AppSettings(
            IDatabaseService databaseService,
            ILogger<AppSettings> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
            _settings = new UserSettings();
        }

        public async Task LoadAsync()
        {
            try
            {
                _settings = await _databaseService.GetUserSettingsAsync();
                _logger.LogInformation("Settings loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading settings");
                _settings = new UserSettings(); // Use default settings in case of error
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await _databaseService.SaveUserSettingsAsync(_settings);
                _logger.LogInformation("Settings saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings");
                throw;
            }
        }

        public T GetSetting<T>(string key, T defaultValue = default)
        {
            switch (key)
            {
                case "Username":
                    return (T)(object)_settings.Username;
                case "MedicalLicenseNumber":
                    return (T)(object)_settings.MedicalLicenseNumber;
                case "TrainingUnit":
                    return (T)(object)_settings.TrainingUnit;
                case "Supervisor":
                    return (T)(object)_settings.Supervisor;
                case "EnableNotifications":
                    return (T)(object)_settings.EnableNotifications;
                case "EnableAutoSync":
                    return (T)(object)_settings.EnableAutoSync;
                case "UseDarkTheme":
                    return (T)(object)_settings.UseDarkTheme;
                default:
                    return defaultValue;
            }
        }

        public void SetSetting<T>(string key, T value)
        {
            switch (key)
            {
                case "Username":
                    _settings.Username = value as string;
                    break;
                case "MedicalLicenseNumber":
                    _settings.MedicalLicenseNumber = value as string;
                    break;
                case "TrainingUnit":
                    _settings.TrainingUnit = value as string;
                    break;
                case "Supervisor":
                    _settings.Supervisor = value as string;
                    break;
                case "EnableNotifications":
                    if (value is bool boolValue)
                        _settings.EnableNotifications = boolValue;
                    break;
                case "EnableAutoSync":
                    if (value is bool autoSyncValue)
                        _settings.EnableAutoSync = autoSyncValue;
                    break;
                case "UseDarkTheme":
                    if (value is bool darkThemeValue)
                        _settings.UseDarkTheme = darkThemeValue;
                    break;
            }
        }

        public void RemoveSetting(string key)
        {
            SetSetting<object>(key, null);
        }
    }
}