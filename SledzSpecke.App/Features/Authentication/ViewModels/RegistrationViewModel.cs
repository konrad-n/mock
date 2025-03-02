using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services;
using SledzSpecke.App.Services.Interfaces;
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
            this._dataManager = dataManager;
            this._authenticationService = authenticationService;
            this.SpecializationTypes = new List<SpecializationType>();
            this.Title = "Rejestracja";
            this._navigationService = navigationService;
        }

        public override async Task InitializeAsync()
        {
            await this.LoadSpecializationTypesAsync();
        }

        private async Task LoadSpecializationTypesAsync()
        {
            try
            {
                this.SpecializationTypes = await this._dataManager.GetAllSpecializationTypesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error loading specialization types");
                this.ErrorMessage = $"Nie udało się załadować listy specjalizacji: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(this.Username) ||
                string.IsNullOrWhiteSpace(this.Email) ||
                string.IsNullOrWhiteSpace(this.Password) ||
                string.IsNullOrWhiteSpace(this.ConfirmPassword) ||
                this.SpecializationSelectedIndex == -1)
            {
                this.ErrorMessage = "Proszę wypełnić wszystkie pola formularza.";
                return;
            }

            if (this.Password != this.ConfirmPassword)
            {
                this.ErrorMessage = "Hasła nie są identyczne.";
                return;
            }

            this.IsLoading = true;
            this.ErrorMessage = string.Empty;

            try
            {
                int specializationTypeId = this.SpecializationTypes[this.SpecializationSelectedIndex].Id;

                bool result = await this._authenticationService.RegisterAsync(
                    this.Username,
                    this.Email,
                    this.Password,
                    specializationTypeId);

                if (result)
                {
                    await this._navigationService.DisplayAlertAsync(
                        "Sukces",
                        "Rejestracja zakończona pomyślnie. Możesz się teraz zalogować.",
                        "OK");

                    await this._navigationService.PopAsync();
                }
                else
                {
                    this.ErrorMessage = "Użytkownik o podanym adresie email już istnieje.";
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error during registration");
                this.ErrorMessage = $"Wystąpił problem podczas rejestracji: {ex.Message}";
            }
            finally
            {
                this.IsLoading = false;
            }
        }
    }
}