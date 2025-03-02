using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Services.Interfaces;

namespace SledzSpecke.App.Features.Authentication.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly INavigationService navigationService;
        private readonly ISpecializationService specializationService;
        private readonly IServiceProvider serviceProvider;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public LoginViewModel(
            IAuthenticationService authenticationService,
            ILogger<LoginViewModel> logger,
            ISpecializationService specializationService,
            IServiceProvider serviceProvider,
            INavigationService navigationService) : base(logger)
        {
            this.authenticationService = authenticationService;
            this.specializationService = specializationService;
            this.serviceProvider = serviceProvider;
            this.navigationService = navigationService;

            // Dodaj debug log
            System.Diagnostics.Debug.WriteLine("LoginViewModel constructor called");

#if DEBUG
            this.Email = "olo@pozakontrololo.com";
            this.Password = "gucio";
            System.Diagnostics.Debug.WriteLine($"Debug values set: Email = {this.Email}, Password = {this.Password}");
#endif
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Email) || string.IsNullOrWhiteSpace(this.Password))
            {
                this.ErrorMessage = "Proszę wprowadzić adres email i hasło.";
                return;
            }

            try
            {
                this.IsLoading = true;
                this.ErrorMessage = string.Empty;

                this.logger.LogInformation("Login attempt for {Email}", this.Email);

                bool result = await this.authenticationService.LoginAsync(this.Email, this.Password);

                if (result)
                {
                    this.logger.LogInformation("Login successful for {Email}", this.Email);

                    try
                    {
                        var appShell = new AppShell(this.authenticationService, this.specializationService, this.serviceProvider);
                        Application.Current.MainPage = appShell;
                        this.logger.LogDebug("AppShell set as MainPage");
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Error setting AppShell as MainPage");
                        this.ErrorMessage = "Logowanie powiodło się, ale wystąpił problem z uruchomieniem głównego ekranu aplikacji.";
                    }
                }
                else
                {
                    this.logger.LogWarning("Login failed for {Email}", this.Email);
                    this.ErrorMessage = "Nieprawidłowy adres email lub hasło.";
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during login for {Email}", this.Email);
                this.ErrorMessage = $"Wystąpił problem podczas próby logowania. Szczegóły błędu: {ex.Message}";
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToRegistrationAsync()
        {
            await this.navigationService.NavigateToAsync<RegistrationPage>();
        }
    }
}