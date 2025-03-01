using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.Authentication.Views;
using SledzSpecke.App.Services;

namespace SledzSpecke.App.Features.Authentication.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;
        private ISpecializationService _specializationService;
        private IServiceProvider _serviceProvider;

        public LoginViewModel(
            IAuthenticationService authenticationService,
            ILogger<LoginViewModel> logger,
            ISpecializationService specializationService,
            IServiceProvider serviceProvider,
            INavigationService navigationService) : base(logger)
        {
            _authenticationService = authenticationService;
            _serviceProvider = serviceProvider;
            _specializationService = specializationService;
            _navigationService = navigationService;

            // Dodaj debug log
            System.Diagnostics.Debug.WriteLine("LoginViewModel constructor called");

#if DEBUG
            Email = "olo@pozakontrololo.com";
            Password = "gucio";
            System.Diagnostics.Debug.WriteLine($"Debug values set: Email = {Email}, Password = {Password}");
#endif
            _specializationService = specializationService;
            _serviceProvider = serviceProvider;
            _navigationService = navigationService;
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
                        var appShell = new AppShell(_authenticationService, _specializationService, _serviceProvider);
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
            await _navigationService.NavigateToAsync<RegistrationPage>();
        }
    }
}