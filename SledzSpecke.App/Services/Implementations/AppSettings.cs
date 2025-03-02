using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class AppSettings : IAppSettings
    {
        private readonly IDatabaseService databaseService;
        private readonly ILogger<AppSettings> logger;
        private UserSettings _settings;

        public AppSettings(
            IDatabaseService databaseService,
            ILogger<AppSettings> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
            this._settings = new UserSettings();
        }

        public async Task LoadAsync()
        {
            try
            {
                this._settings = await this.databaseService.GetUserSettingsAsync();
                this.logger.LogInformation("Settings loaded successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading settings");
                this._settings = new UserSettings(); // Use default settings in case of error
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await this.databaseService.SaveUserSettingsAsync(this._settings);
                this.logger.LogInformation("Settings saved successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving settings");
                throw;
            }
        }

        public T GetSetting<T>(string key, T defaultValue = default)
        {
            switch (key)
            {
                case "Username":
                    return (T)(object)this._settings.Username;
                case "MedicalLicenseNumber":
                    return (T)(object)this._settings.MedicalLicenseNumber;
                case "TrainingUnit":
                    return (T)(object)this._settings.TrainingUnit;
                case "Supervisor":
                    return (T)(object)this._settings.Supervisor;
                case "EnableNotifications":
                    return (T)(object)this._settings.EnableNotifications;
                case "EnableAutoSync":
                    return (T)(object)this._settings.EnableAutoSync;
                case "UseDarkTheme":
                    return (T)(object)this._settings.UseDarkTheme;
                default:
                    return defaultValue;
            }
        }

        public void SetSetting<T>(string key, T value)
        {
            switch (key)
            {
                case "Username":
                    this._settings.Username = value as string;
                    break;
                case "MedicalLicenseNumber":
                    this._settings.MedicalLicenseNumber = value as string;
                    break;
                case "TrainingUnit":
                    this._settings.TrainingUnit = value as string;
                    break;
                case "Supervisor":
                    this._settings.Supervisor = value as string;
                    break;
                case "EnableNotifications":
                    if (value is bool boolValue)
                        this._settings.EnableNotifications = boolValue;
                    break;
                case "EnableAutoSync":
                    if (value is bool autoSyncValue)
                        this._settings.EnableAutoSync = autoSyncValue;
                    break;
                case "UseDarkTheme":
                    if (value is bool darkThemeValue)
                        this._settings.UseDarkTheme = darkThemeValue;
                    break;
            }
        }

        public void RemoveSetting(string key)
        {
            this.SetSetting<object>(key, null);
        }
    }
}