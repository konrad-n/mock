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
        private Specialization _specialization;

        [ObservableProperty]
        private string _specializationName;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private string _durationYears;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _medicalLicenseNumber;

        [ObservableProperty]
        private string _trainingUnit;

        [ObservableProperty]
        private string _supervisor;

        [ObservableProperty]
        private bool _enableNotifications;

        [ObservableProperty]
        private bool _enableAutoSync;

        [ObservableProperty]
        private bool _useDarkTheme;

        public SettingsViewModel(
            IAppSettings appSettings,
            IDataManager dataManager,
            ILogger<SettingsViewModel> logger) : base(logger)
        {
            this.appSettings = appSettings;
            this.dataManager = dataManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                this._specialization = await this.dataManager.LoadSpecializationAsync();
                if (this._specialization == null)
                {
                    throw new InvalidOperationException("Nie udało się załadować danych specjalizacji.");
                }

                // Dane specjalizacji
                this.SpecializationName = this._specialization.Name;
                this.StartDate = this._specialization.StartDate;
                this.DurationYears = ((this._specialization.ExpectedEndDate - this._specialization.StartDate).Days / 365).ToString();

                // Dane użytkownika
                this.Username = this.appSettings.GetSetting<string>("Username", "");
                this.MedicalLicenseNumber = this.appSettings.GetSetting<string>("MedicalLicenseNumber", "");
                this.TrainingUnit = this.appSettings.GetSetting<string>("TrainingUnit", "");
                this.Supervisor = this.appSettings.GetSetting<string>("Supervisor", "");

                // Ustawienia aplikacji
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
                throw new InvalidOperationException("Imię i nazwisko jest wymagane.");

            if (string.IsNullOrWhiteSpace(this.MedicalLicenseNumber))
                throw new InvalidOperationException("Numer PWZ jest wymagany.");

            try
            {
                // Aktualizacja danych specjalizacji
                this._specialization.StartDate = this.StartDate;
                this._specialization.ExpectedEndDate = this.StartDate.AddYears(5);

                // Zapisanie specialization
                await this.dataManager.SaveSpecializationAsync(this._specialization);

                // Aktualizacja ustawień użytkownika
                this.appSettings.SetSetting("Username", this.Username);
                this.appSettings.SetSetting("MedicalLicenseNumber", this.MedicalLicenseNumber);
                this.appSettings.SetSetting("TrainingUnit", this.TrainingUnit);
                this.appSettings.SetSetting("Supervisor", this.Supervisor);

                // Aktualizacja ustawień aplikacji
                this.appSettings.SetSetting("EnableNotifications", this.EnableNotifications);
                this.appSettings.SetSetting("EnableAutoSync", this.EnableAutoSync);
                this.appSettings.SetSetting("UseDarkTheme", this.UseDarkTheme);

                // Zapisanie ustawień
                await this.appSettings.SaveAsync();

                // Zastosowanie ustawień motywu
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
                    throw new Exception("Failed to delete data");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error clearing data");
                throw;
            }
        }
    }
}
