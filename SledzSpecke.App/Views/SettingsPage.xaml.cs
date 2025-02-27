using System;
using Microsoft.Maui.Controls;
using SledzSpecke.Core.Models;
using SledzSpecke.Services;

namespace SledzSpecke.Views
{
    public partial class SettingsPage : ContentPage
    {
        private Specialization _specialization;



        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private async void LoadSettings()
        {
            try
            {
                _specialization = await App.DataManager.LoadSpecializationAsync();

                // Załadowanie danych specjalizacji
                SpecializationNameEntry.Text = _specialization.Name;
                StartDatePicker.Date = _specialization.StartDate;
                DurationYearsEntry.Text = (_specialization.BaseDurationWeeks / 52.0).ToString("F1");

                // Załadowanie zapisanych danych personalnych
                FullNameEntry.Text = App.AppSettings.GetSetting<string>("Username", "");
                MedicalLicenseNumberEntry.Text = App.AppSettings.GetSetting<string>("MedicalLicenseNumber", "");
                TrainingUnitEntry.Text = App.AppSettings.GetSetting<string>("TrainingUnit", "");
                SupervisorEntry.Text = App.AppSettings.GetSetting<string>("Supervisor", "");

                // Załadowanie ustawień aplikacji
                NotificationsSwitch.IsToggled = App.AppSettings.GetSetting<bool>("EnableNotifications", true);
                AutoSyncSwitch.IsToggled = App.AppSettings.GetSetting<bool>("EnableAutoSync", true);
                DarkThemeSwitch.IsToggled = App.AppSettings.GetSetting<bool>("UseDarkTheme", false);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem podczas ładowania ustawień: {ex.Message}", "OK");
            }
        }

        private async void OnConfigureSMKClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Informacja", "Funkcja konfiguracji integracji z SMK zostanie zaimplementowana w przyszłej wersji aplikacji.", "OK");
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
            {
                await DisplayAlert("Błąd", "Imię i nazwisko jest wymagane.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(MedicalLicenseNumberEntry.Text))
            {
                await DisplayAlert("Błąd", "Numer PWZ jest wymagany.", "OK");
                return;
            }

            try
            {
                // Aktualizacja danych specjalizacji
                _specialization.StartDate = StartDatePicker.Date;
                _specialization.ExpectedEndDate = StartDatePicker.Date.AddYears(5);

                // Zapisanie specialization
                await App.DataManager.SaveSpecializationAsync(_specialization);

                // Aktualizacja ustawień użytkownika
                App.AppSettings.SetSetting("Username", FullNameEntry.Text);
                App.AppSettings.SetSetting("MedicalLicenseNumber", MedicalLicenseNumberEntry.Text);
                App.AppSettings.SetSetting("TrainingUnit", TrainingUnitEntry.Text);
                App.AppSettings.SetSetting("Supervisor", SupervisorEntry.Text);

                // Aktualizacja ustawień aplikacji
                App.AppSettings.SetSetting("EnableNotifications", NotificationsSwitch.IsToggled);
                App.AppSettings.SetSetting("EnableAutoSync", AutoSyncSwitch.IsToggled);
                App.AppSettings.SetSetting("UseDarkTheme", DarkThemeSwitch.IsToggled);

                // Zastosowanie ustawień motywu
                Application.Current.UserAppTheme = DarkThemeSwitch.IsToggled ? AppTheme.Dark : AppTheme.Light;

                // Zapisanie ustawień
                await App.AppSettings.SaveAsync();

                await DisplayAlert("Sukces", "Ustawienia zostały zapisane pomyślnie.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem podczas zapisywania ustawień: {ex.Message}", "OK");
            }
        }

        private async void OnClearDataClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Potwierdzenie", "Czy na pewno chcesz wyczyścić wszystkie dane? Ta operacja jest nieodwracalna.", "Tak", "Nie");

            if (answer)
            {
                try
                {
                    bool success = await App.DataManager.DeleteAllDataAsync();

                    if (success)
                    {
                        await DisplayAlert("Sukces", "Wszystkie dane zostały wyczyszczone pomyślnie.", "OK");

                        // Powrót do strony startowej
                        await Navigation.PopToRootAsync();
                    }
                    else
                    {
                        await DisplayAlert("Błąd", "Wystąpił problem podczas czyszczenia danych.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Błąd", $"Wystąpił problem podczas czyszczenia danych: {ex.Message}", "OK");
                }
            }
        }
    }
}