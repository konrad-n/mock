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
        private readonly IAppSettings _appSettings;
        private readonly IDataManager _dataManager;
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
            _appSettings = appSettings;
            _dataManager = dataManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _specialization = await _dataManager.LoadSpecializationAsync();

                // Załadowanie danych specjalizacji
                SpecializationName = _specialization.Name;
                StartDate = _specialization.StartDate;
                DurationYears = (_specialization.BaseDurationWeeks / 52.0).ToString("F1");

                // Załadowanie zapisanych danych personalnych
                Username = _appSettings.GetSetting<string>("Username", "");
                MedicalLicenseNumber = _appSettings.GetSetting<string>("MedicalLicenseNumber", "");
                TrainingUnit = _appSettings.GetSetting<string>("TrainingUnit", "");
                Supervisor = _appSettings.GetSetting<string>("Supervisor", "");

                // Załadowanie ustawień aplikacji
                EnableNotifications = _appSettings.GetSetting<bool>("EnableNotifications", true);
                EnableAutoSync = _appSettings.GetSetting<bool>("EnableAutoSync", true);
                UseDarkTheme = _appSettings.GetSetting<bool>("UseDarkTheme", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading settings");
                throw;
            }
        }

        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (string.IsNullOrWhiteSpace(Username))
                throw new InvalidOperationException("Imię i nazwisko jest wymagane.");

            if (string.IsNullOrWhiteSpace(MedicalLicenseNumber))
                throw new InvalidOperationException("Numer PWZ jest wymagany.");

            try
            {
                // Aktualizacja danych specjalizacji
                _specialization.StartDate = StartDate;
                _specialization.ExpectedEndDate = StartDate.AddYears(5);

                // Zapisanie specialization
                await _dataManager.SaveSpecializationAsync(_specialization);

                // Aktualizacja ustawień użytkownika
                _appSettings.SetSetting("Username", Username);
                _appSettings.SetSetting("MedicalLicenseNumber", MedicalLicenseNumber);
                _appSettings.SetSetting("TrainingUnit", TrainingUnit);
                _appSettings.SetSetting("Supervisor", Supervisor);

                // Aktualizacja ustawień aplikacji
                _appSettings.SetSetting("EnableNotifications", EnableNotifications);
                _appSettings.SetSetting("EnableAutoSync", EnableAutoSync);
                _appSettings.SetSetting("UseDarkTheme", UseDarkTheme);

                // Zapisanie ustawień
                await _appSettings.SaveAsync();

                // Zastosowanie ustawień motywu
                Application.Current.UserAppTheme = UseDarkTheme ? AppTheme.Dark : AppTheme.Light;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings");
                throw;
            }
        }

        [RelayCommand]
        public async Task ClearDataAsync()
        {
            try
            {
                bool success = await _dataManager.DeleteAllDataAsync();
                if (!success)
                    throw new Exception("Failed to delete data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing data");
                throw;
            }
        }
    }
}
