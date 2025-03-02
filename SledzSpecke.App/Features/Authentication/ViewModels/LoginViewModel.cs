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
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly ISpecializationService _specializationService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        public LoginViewModel(
            IAuthenticationService authenticationService,
            ILogger<LoginViewModel> logger,
            ISpecializationService specializationService,
            IServiceProvider serviceProvider,
            INavigationService navigationService) : base(logger)
        {
            this._authenticationService = authenticationService;
            this._specializationService = specializationService;
            this._serviceProvider = serviceProvider;
            this._navigationService = navigationService;

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

                this._logger.LogInformation("Login attempt for {Email}", this.Email);

                bool result = await this._authenticationService.LoginAsync(this.Email, this.Password);

                if (result)
                {
                    this._logger.LogInformation("Login successful for {Email}", this.Email);

                    try
                    {
                        var appShell = new AppShell(this._authenticationService, this._specializationService, this._serviceProvider);
                        Application.Current.MainPage = appShell;
                        this._logger.LogDebug("AppShell set as MainPage");
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Error setting AppShell as MainPage");
                        this.ErrorMessage = "Logowanie powiodło się, ale wystąpił problem z uruchomieniem głównego ekranu aplikacji.";
                    }
                }
                else
                {
                    this._logger.LogWarning("Login failed for {Email}", this.Email);
                    this.ErrorMessage = "Nieprawidłowy adres email lub hasło.";
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error during login for {Email}", this.Email);
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
            await this._navigationService.NavigateToAsync<RegistrationPage>();
        }
    }
}