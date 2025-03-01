using Microsoft.Extensions.Logging;

namespace SledzSpecke.App.Views.Auth
{
    public partial class LoginPage : ContentPage
    {
        private static readonly ILogger<LoginPage> _logger = LoggerFactory.Create(builder =>
            builder.AddDebug()).CreateLogger<LoginPage>();

        public LoginPage()
        {
            try
            {
                InitializeComponent();
#if DEBUG
                EmailEntry.Text = "olo@pozakontrololo.com";
                PasswordEntry.Text = "gucio";
#endif
                _logger.LogInformation("LoginPage initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing LoginPage");
            }
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                await DisplayAlert("Błąd", "Proszę wprowadzić adres email i hasło.", "OK");
                return;
            }

            // Zabezpieczenie UI przed wielokrotnym kliknięciem
            LoginButton.IsEnabled = false;
            ActivityIndicator.IsRunning = true;

            try
            {
                _logger.LogInformation("Login attempt for {Email}", EmailEntry.Text);

                bool result = await App.AuthenticationService.LoginAsync(EmailEntry.Text, PasswordEntry.Text);

                if (result)
                {
                    _logger.LogInformation("Login successful for {Email}", EmailEntry.Text);

                    try
                    {
                        // Utwórz i ustaw nową instancję AppShell
                        var appShell = new AppShell();
                        Application.Current.MainPage = appShell;
                        _logger.LogDebug("AppShell set as MainPage");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error setting AppShell as MainPage");
                        await DisplayAlert("Błąd aplikacji",
                            "Logowanie powiodło się, ale wystąpił problem z uruchomieniem głównego ekranu aplikacji. " +
                            $"Szczegóły: {ex.Message}", "OK");
                    }
                }
                else
                {
                    _logger.LogWarning("Login failed for {Email}", EmailEntry.Text);
                    await DisplayAlert("Błąd logowania", "Nieprawidłowy adres email lub hasło.", "OK");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", EmailEntry.Text);

                await DisplayAlert("Błąd logowania",
                    "Wystąpił problem podczas próby logowania. " +
                    $"Szczegóły błędu: {ex.Message}", "OK");

                // Dodatkowe informacje diagnostyczne
                var innerMsg = ex.InnerException != null ? $"\nWewnętrzny błąd: {ex.InnerException.Message}" : "";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}{innerMsg}\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                // Zawsze włącz z powrotem przycisk i zatrzymaj indykator
                LoginButton.IsEnabled = true;
                ActivityIndicator.IsRunning = false;
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new RegistrationPage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to RegistrationPage");
                await DisplayAlert("Błąd nawigacji",
                    "Nie można przejść do ekranu rejestracji. " +
                    $"Szczegóły: {ex.Message}", "OK");
            }
        }
    }
}