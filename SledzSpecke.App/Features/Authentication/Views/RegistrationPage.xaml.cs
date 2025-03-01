using SledzSpecke.App.Services;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Authentication.Views
{
    public partial class RegistrationPage : ContentPage
    {
        private List<SpecializationType> _specializationTypes;
        private IDataManager _dataManager;
        private IAuthenticationService _authenticationService;

        public RegistrationPage()
        {
            _dataManager = App.DataManager;
            _authenticationService = App.AuthenticationService;

            InitializeComponent();
            LoadSpecializationTypes();
        }

        private async void LoadSpecializationTypes()
        {
            try
            {
                _specializationTypes = await _dataManager.GetAllSpecializationTypesAsync();

                foreach (var type in _specializationTypes)
                {
                    SpecializationPicker.Items.Add(type.Name);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Nie udało się załadować listy specjalizacji: {ex.Message}", "OK");
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text) ||
                SpecializationPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Błąd", "Proszę wypełnić wszystkie pola formularza.", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Błąd", "Hasła nie są identyczne.", "OK");
                return;
            }

            RegisterButton.IsEnabled = false;
            ActivityIndicator.IsRunning = true;

            try
            {
                int specializationTypeId = _specializationTypes[SpecializationPicker.SelectedIndex].Id;

                bool result = await _authenticationService.RegisterAsync(
                    UsernameEntry.Text,
                    EmailEntry.Text,
                    PasswordEntry.Text,
                    specializationTypeId);

                if (result)
                {
                    await DisplayAlert("Sukces", "Rejestracja zakończona pomyślnie. Możesz się teraz zalogować.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Błąd rejestracji", "Użytkownik o podanym adresie email już istnieje.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem podczas rejestracji: {ex.Message}", "OK");
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                ActivityIndicator.IsRunning = false;
            }
        }
    }
}