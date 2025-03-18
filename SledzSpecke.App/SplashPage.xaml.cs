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

            bool isAuthenticated = await this.authService.IsAuthenticatedAsync();

            Page mainPage;
            if (isAuthenticated)
            {
                mainPage = new AppShell(this.authService);
            }
            else
            {
                var viewModel = IPlatformApplication.Current.Services.GetService<LoginViewModel>();
                var loginPage = new LoginPage(viewModel);
                mainPage = new NavigationPage(loginPage);
            }

            var windows = Application.Current?.Windows;
            if (windows != null && windows.Count > 0)
            {
                var window = windows[0];
                window.Page = mainPage;
            }
        }
    }
}