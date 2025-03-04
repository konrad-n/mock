using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.ViewModels.Authentication;
using SledzSpecke.App.Views.Authentication;

namespace SledzSpecke.App
{
    public partial class SplashPage : ContentPage
    {
        private readonly IAuthService authService;

        public SplashPage(IAuthService authService)
        {
            this.authService = authService;
            this.InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Symulacja procesu inicjalizacji
            await Task.Delay(2000);

            try
            {
                // Sprawdzenie, czy użytkownik jest zalogowany
                bool isAuthenticated = await this.authService.IsAuthenticatedAsync();

                // Określenie, którą stronę pokazać
                Page mainPage;
                if (isAuthenticated)
                {
                    mainPage = new AppShell(this.authService);
                }
                else
                {
                    // Przygotowanie ViewModel dla ekranu logowania
                    var viewModel = IPlatformApplication.Current.Services.GetService<LoginViewModel>();
                    var loginPage = new LoginPage(viewModel);
                    mainPage = new NavigationPage(loginPage);
                }

                // Ustawienie głównej strony
                var windows = Application.Current?.Windows;
                if (windows != null && windows.Count > 0)
                {
                    var window = windows[0];
                    window.Page = mainPage;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas przechodzenia z ekranu startowego: {ex.Message}");
                await this.DisplayAlert("Błąd", "Nie udało się zainicjalizować aplikacji. Spróbuj ponownie.", "OK");
            }
        }
    }
}