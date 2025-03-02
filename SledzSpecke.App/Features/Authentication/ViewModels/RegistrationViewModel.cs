using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Authentication.ViewModels
{
    public partial class RegistrationViewModel : ViewModelBase
    {
        private readonly IDataManager dataManager;
        private readonly IAuthenticationService authenticationService;
        private readonly INavigationService navigationService;

        [ObservableProperty]
        private List<SpecializationType> specializationTypes;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private int specializationSelectedIndex = -1;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public RegistrationViewModel(
            IDataManager dataManager,
            IAuthenticationService authenticationService,
            ILogger<RegistrationViewModel> logger,
            INavigationService navigationService)
            : base(logger)
        {
            this.dataManager = dataManager;
            this.authenticationService = authenticationService;
            this.SpecializationTypes = new List<SpecializationType>();
            this.Title = "Rejestracja";
            this.navigationService = navigationService;
        }

        public override async Task InitializeAsync()
        {
            await this.LoadSpecializationTypesAsync();
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
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

                bool result = await this.authenticationService.RegisterAsync(
                    this.Username,
                    this.Email,
                    this.Password,
                    specializationTypeId);

                if (result)
                {
                    await this.navigationService.DisplayAlertAsync(
                        "Sukces",
                        "Rejestracja zakończona pomyślnie. Możesz się teraz zalogować.",
                        "OK");

                    await this.navigationService.PopAsync();
                }
                else
                {
                    this.ErrorMessage = "Użytkownik o podanym adresie email już istnieje.";
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during registration");
                this.ErrorMessage = $"Wystąpił problem podczas rejestracji: {ex.Message}";
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        private async Task LoadSpecializationTypesAsync()
        {
            try
            {
                this.SpecializationTypes = await this.dataManager.GetAllSpecializationTypesAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading specialization types");
                this.ErrorMessage = $"Nie udało się załadować listy specjalizacji: {ex.Message}";
            }
        }
    }
}