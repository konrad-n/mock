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

        private string username = string.Empty;
        private string password = string.Empty;
        private bool rememberMe;

        public LoginViewModel(IAuthService authService, IDialogService dialogService)
        {
            this.authService = authService;
            this.dialogService = dialogService;

            this.Title = "Logowanie";

            // Inicjalizacja komend
            this.LoginCommand = new AsyncRelayCommand(this.OnLoginAsync, this.CanLogin);
            this.GoToRegisterCommand = new AsyncRelayCommand(this.OnGoToRegisterAsync);
        }

        // Właściwości do bindowania
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

        // Komendy
        public ICommand LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }

        // Metody pomocnicze
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

            try
            {
                bool success = await this.authService.LoginAsync(this.Username, this.Password);

                if (success)
                {
                    // Przejście do głównej części aplikacji
                    Application.Current.MainPage = new AppShell(this.authService);
                }
                else
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd logowania",
                        "Nieprawidłowa nazwa użytkownika lub hasło.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas logowania: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił błąd podczas logowania. Spróbuj ponownie później.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnGoToRegisterAsync()
        {
            try
            {
                if (this.IsBusy)
                    return;

                this.IsBusy = true;

                // Przejście do strony rejestracji
                if (Application.Current.MainPage is NavigationPage navigationPage)
                {
                    // Tworzenie ViewModel z DI zamiast bezpośrednio
                    var registerViewModel = IPlatformApplication.Current.Services.GetRequiredService<RegisterViewModel>();

                    // Tworzymy stronę i przechodzimy do niej
                    var registerPage = new Views.Authentication.RegisterPage(registerViewModel);
                    await navigationPage.PushAsync(registerPage);

                    // Inicjalizacja MUSI zostać wykonana po przejściu do strony, nie tutaj
                    // (RegisterPage wywoła InitializeAsync w OnAppearing)
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w OnGoToRegisterAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem z przejściem do strony rejestracji.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}