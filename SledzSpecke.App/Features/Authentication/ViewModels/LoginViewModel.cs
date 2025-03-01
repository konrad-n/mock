using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services;

namespace SledzSpecke.App.Features.Authentication.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;

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
            ILogger<LoginViewModel> logger) : base(logger)
        {
            _authenticationService = authenticationService;
            Title = "Logowanie";

#if DEBUG
            Email = "olo@pozakontrololo.com";
            Password = "gucio";
#endif
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Proszę wprowadzić adres email i hasło.";
                return;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                _logger.LogInformation("Login attempt for {Email}", Email);

                bool result = await _authenticationService.LoginAsync(Email, Password);

                if (result)
                {
                    _logger.LogInformation("Login successful for {Email}", Email);

                    try
                    {
                        var appShell = new AppShell();
                        Application.Current.MainPage = appShell;
                        _logger.LogDebug("AppShell set as MainPage");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error setting AppShell as MainPage");
                        ErrorMessage = "Logowanie powiodło się, ale wystąpił problem z uruchomieniem głównego ekranu aplikacji.";
                    }
                }
                else
                {
                    _logger.LogWarning("Login failed for {Email}", Email);
                    ErrorMessage = "Nieprawidłowy adres email lub hasło.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", Email);
                ErrorMessage = $"Wystąpił problem podczas próby logowania. Szczegóły błędu: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToRegistrationAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("//registration");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to RegistrationPage");
                ErrorMessage = "Nie można przejść do ekranu rejestracji.";
            }
        }
    }
}