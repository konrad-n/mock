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
        private UserSettings settings;

        public AppSettings(
            IDatabaseService databaseService,
            ILogger<AppSettings> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
            this.settings = new UserSettings();
        }

        public async Task LoadAsync()
        {
            try
            {
                var loadedSettings = await this.databaseService.GetUserSettingsAsync();
                this.settings = loadedSettings ?? new UserSettings();
                this.logger.LogInformation("Settings loaded successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading settings");
                this.settings = new UserSettings(); // Use default settings in case of error
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await this.databaseService.SaveUserSettingsAsync(this.settings);
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
            // Use pattern matching and null coalescing to handle potential nulls
            return key switch
            {
                "Username" => this.settings.Username is not null && typeof(T) == typeof(string)
                    ? (T)(object)this.settings.Username
                    : defaultValue,

                "MedicalLicenseNumber" => this.settings.MedicalLicenseNumber is not null && typeof(T) == typeof(string)
                    ? (T)(object)this.settings.MedicalLicenseNumber
                    : defaultValue,

                "TrainingUnit" => this.settings.TrainingUnit is not null && typeof(T) == typeof(string)
                    ? (T)(object)this.settings.TrainingUnit
                    : defaultValue,

                "Supervisor" => this.settings.Supervisor is not null && typeof(T) == typeof(string)
                    ? (T)(object)this.settings.Supervisor
                    : defaultValue,

                "EnableNotifications" => typeof(T) == typeof(bool)
                    ? (T)(object)this.settings.EnableNotifications
                    : defaultValue,

                "EnableAutoSync" => typeof(T) == typeof(bool)
                    ? (T)(object)this.settings.EnableAutoSync
                    : defaultValue,

                "UseDarkTheme" => typeof(T) == typeof(bool)
                    ? (T)(object)this.settings.UseDarkTheme
                    : defaultValue,

                _ => defaultValue
            };
        }

        public void SetSetting<T>(string key, T value)
        {
            switch (key)
            {
                case "Username":
                    // Use null-conditional operator to avoid null assignments
                    if (value is string username)
                    {
                        this.settings.Username = username;
                    }

                    break;

                case "MedicalLicenseNumber":
                    if (value is string licenseNumber)
                    {
                        this.settings.MedicalLicenseNumber = licenseNumber;
                    }

                    break;

                case "TrainingUnit":
                    if (value is string trainingUnit)
                    {
                        this.settings.TrainingUnit = trainingUnit;
                    }

                    break;

                case "Supervisor":
                    if (value is string supervisor)
                    {
                        this.settings.Supervisor = supervisor;
                    }

                    break;

                case "EnableNotifications":
                    if (value is bool boolValue)
                    {
                        this.settings.EnableNotifications = boolValue;
                    }

                    break;

                case "EnableAutoSync":
                    if (value is bool autoSyncValue)
                    {
                        this.settings.EnableAutoSync = autoSyncValue;
                    }

                    break;

                case "UseDarkTheme":
                    if (value is bool darkThemeValue)
                    {
                        this.settings.UseDarkTheme = darkThemeValue;
                    }

                    break;
            }
        }

        public void RemoveSetting(string key)
        {
            // Handle string properties separately from bool properties
            switch (key)
            {
                case "Username":
                    this.settings.Username = string.Empty;
                    break;

                case "MedicalLicenseNumber":
                    this.settings.MedicalLicenseNumber = string.Empty;
                    break;

                case "TrainingUnit":
                    this.settings.TrainingUnit = string.Empty;
                    break;

                case "Supervisor":
                    this.settings.Supervisor = string.Empty;
                    break;

                case "EnableNotifications":
                    this.settings.EnableNotifications = false;
                    break;

                case "EnableAutoSync":
                    this.settings.EnableAutoSync = false;
                    break;

                case "UseDarkTheme":
                    this.settings.UseDarkTheme = false;
                    break;
            }
        }
    }
}