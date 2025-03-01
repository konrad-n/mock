using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Authentication.ViewModels
{
    public partial class RegistrationViewModel : ViewModelBase
    {
        private readonly IDataManager _dataManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private List<SpecializationType> _specializationTypes;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private int _specializationSelectedIndex = -1;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        public RegistrationViewModel(
            IDataManager dataManager,
            IAuthenticationService authenticationService,
            ILogger<RegistrationViewModel> logger,
            INavigationService navigationService) : base(logger)
        {
            _dataManager = dataManager;
            _authenticationService = authenticationService;
            SpecializationTypes = new List<SpecializationType>();
            Title = "Rejestracja";
            _navigationService = navigationService;
        }

        public override async Task InitializeAsync()
        {
            await LoadSpecializationTypesAsync();
        }

        private async Task LoadSpecializationTypesAsync()
        {
            try
            {
                SpecializationTypes = await _dataManager.GetAllSpecializationTypesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading specialization types");
                ErrorMessage = $"Nie udało się załadować listy specjalizacji: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword) ||
                SpecializationSelectedIndex == -1)
            {
                ErrorMessage = "Proszę wypełnić wszystkie pola formularza.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Hasła nie są identyczne.";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                int specializationTypeId = SpecializationTypes[SpecializationSelectedIndex].Id;

                bool result = await _authenticationService.RegisterAsync(
                    Username,
                    Email,
                    Password,
                    specializationTypeId);

                if (result)
                {
                    await _navigationService.DisplayAlertAsync(
                        "Sukces",
                        "Rejestracja zakończona pomyślnie. Możesz się teraz zalogować.",
                        "OK");

                    await _navigationService.PopAsync();
                }
                else
                {
                    ErrorMessage = "Użytkownik o podanym adresie email już istnieje.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ErrorMessage = $"Wystąpił problem podczas rejestracji: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}