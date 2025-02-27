using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services
{
    public class AppSettings
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<AppSettings> _logger;
        private UserSettings _settings;

        public AppSettings(DatabaseService databaseService, ILogger<AppSettings> logger)
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
                    _settings.Username = (string)(object)value;
                    break;
                case "MedicalLicenseNumber":
                    _settings.MedicalLicenseNumber = (string)(object)value;
                    break;
                case "TrainingUnit":
                    _settings.TrainingUnit = (string)(object)value;
                    break;
                case "Supervisor":
                    _settings.Supervisor = (string)(object)value;
                    break;
                case "EnableNotifications":
                    _settings.EnableNotifications = (bool)(object)value;
                    break;
                case "EnableAutoSync":
                    _settings.EnableAutoSync = (bool)(object)value;
                    break;
                case "UseDarkTheme":
                    _settings.UseDarkTheme = (bool)(object)value;
                    break;
            }
        }

        public void RemoveSetting(string key)
        {
            SetSetting(key, default);
        }
    }
}