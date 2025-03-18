using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;

        private string username;
        private string password;
        private bool rememberMe;

        public LoginViewModel(IAuthService authService, IDialogService dialogService)
        {
            this.authService = authService;
            this.dialogService = dialogService;
            this.Title = "Logowanie";
            this.LoginCommand = new AsyncRelayCommand(this.OnLoginAsync, this.CanLogin);
            this.GoToRegisterCommand = new AsyncRelayCommand(this.OnGoToRegisterAsync);
        }

        public string Username
        {
            get => this.username;
            set
            {
                if (this.SetProperty(ref this.username, value))
                {
                    ((AsyncRelayCommand)this.LoginCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => this.password;
            set
            {
                if (this.SetProperty(ref this.password, value))
                {
                    ((AsyncRelayCommand)this.LoginCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool RememberMe
        {
            get => this.rememberMe;
            set => this.SetProperty(ref this.rememberMe, value);
        }

        public ICommand LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(this.Username) &&
                   !string.IsNullOrWhiteSpace(this.Password);
        }

        private async Task OnLoginAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            bool success = await this.authService.LoginAsync(this.Username, this.Password);

            if (success)
            {
                // Przejście do głównej części aplikacji używając kontenera DI
                var appShell = IPlatformApplication.Current.Services.GetService<AppShell>();
                if (appShell != null)
                {
                    Application.Current.MainPage = appShell;
                    System.Diagnostics.Debug.WriteLine("Zalogowano pomyślnie, utworzono AppShell z kontenera DI");
                }
                else
                {
                    // Jeśli z jakiegoś powodu nie możemy pobrać z DI, tworzymy bezpośrednio
                    Application.Current.MainPage = new AppShell(this.authService);
                    System.Diagnostics.Debug.WriteLine("Zalogowano pomyślnie, utworzono AppShell bezpośrednio");
                }
            }
            else
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd logowania",
                    "Nieprawidłowa nazwa użytkownika lub hasło.",
                    "OK");
            }

            this.IsBusy = false;
        }

        private async Task OnGoToRegisterAsync()
        {
            if (this.IsBusy)
                return;

            this.IsBusy = true;

            if (Application.Current.MainPage is NavigationPage navigationPage)
            {
                var registerViewModel = IPlatformApplication.Current.Services.GetRequiredService<RegisterViewModel>();
                var registerPage = new Views.Authentication.RegisterPage(registerViewModel);
                await navigationPage.PushAsync(registerPage);
            }

            this.IsBusy = false;
        }
    }
}