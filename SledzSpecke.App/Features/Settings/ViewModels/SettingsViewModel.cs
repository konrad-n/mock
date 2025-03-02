using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Settings.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly IAppSettings appSettings;
        private readonly IDataManager dataManager;
        private Specialization specialization;

        [ObservableProperty]
        private string specializationName;

        [ObservableProperty]
        private DateTime startDate;

        [ObservableProperty]
        private string durationYears;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string medicalLicenseNumber;

        [ObservableProperty]
        private string trainingUnit;

        [ObservableProperty]
        private string supervisor;

        [ObservableProperty]
        private bool enableNotifications;

        [ObservableProperty]
        private bool enableAutoSync;

        [ObservableProperty]
        private bool useDarkTheme;

        public SettingsViewModel(
            IAppSettings appSettings,
            IDataManager dataManager,
            ILogger<SettingsViewModel> logger)
            : base(logger)
        {
            this.appSettings = appSettings;
            this.dataManager = dataManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                this.specialization = await this.dataManager.LoadSpecializationAsync();
                if (this.specialization == null)
                {
                    throw new InvalidOperationException("Nie udało się załadować danych specjalizacji.");
                }

                this.SpecializationName = this.specialization.Name;
                this.StartDate = this.specialization.StartDate;
                this.DurationYears = ((this.specialization.ExpectedEndDate - this.specialization.StartDate).Days / 365).ToString();
                this.Username = this.appSettings.GetSetting<string>("Username", string.Empty);
                this.MedicalLicenseNumber = this.appSettings.GetSetting<string>("MedicalLicenseNumber", string.Empty);
                this.TrainingUnit = this.appSettings.GetSetting<string>("TrainingUnit", string.Empty);
                this.Supervisor = this.appSettings.GetSetting<string>("Supervisor", string.Empty);
                this.EnableNotifications = this.appSettings.GetSetting<bool>("EnableNotifications", true);
                this.EnableAutoSync = this.appSettings.GetSetting<bool>("EnableAutoSync", true);
                this.UseDarkTheme = this.appSettings.GetSetting<bool>("UseDarkTheme", false);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error initializing settings");
                throw;
            }
        }

        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Username))
            {
                throw new InvalidOperationException("Imię i nazwisko jest wymagane.");
            }

            if (string.IsNullOrWhiteSpace(this.MedicalLicenseNumber))
            {
                throw new InvalidOperationException("Numer PWZ jest wymagany.");
            }

            try
            {
                this.specialization.StartDate = this.StartDate;
                this.specialization.ExpectedEndDate = this.StartDate.AddYears(5);
                await this.dataManager.SaveSpecializationAsync(this.specialization);
                this.appSettings.SetSetting("Username", this.Username);
                this.appSettings.SetSetting("MedicalLicenseNumber", this.MedicalLicenseNumber);
                this.appSettings.SetSetting("TrainingUnit", this.TrainingUnit);
                this.appSettings.SetSetting("Supervisor", this.Supervisor);
                this.appSettings.SetSetting("EnableNotifications", this.EnableNotifications);
                this.appSettings.SetSetting("EnableAutoSync", this.EnableAutoSync);
                this.appSettings.SetSetting("UseDarkTheme", this.UseDarkTheme);
                await this.appSettings.SaveAsync();
                Application.Current.UserAppTheme = this.UseDarkTheme ? AppTheme.Dark : AppTheme.Light;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving settings");
                throw;
            }
        }

        [RelayCommand]
        public async Task ClearDataAsync()
        {
            try
            {
                bool success = await this.dataManager.DeleteAllDataAsync();
                if (!success)
                {
                    throw new Exception("Failed to delete data");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error clearing data");
                throw;
            }
        }
    }
}
