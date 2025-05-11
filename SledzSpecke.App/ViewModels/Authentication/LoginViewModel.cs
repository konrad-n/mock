using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
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

        public LoginViewModel(
            IAuthService authService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
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

            try
            {
                // Validation before attempting login
                if (string.IsNullOrWhiteSpace(Username))
                {
                    throw new InvalidInputException(
                        "Username is required",
                        "Nazwa użytkownika jest wymagana.");
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    throw new InvalidInputException(
                        "Password is required",
                        "Hasło jest wymagane.");
                }

                // Use SafeExecute from BaseViewModel
                bool success = await SafeExecuteAsync(
                    async () => await this.authService.LoginAsync(this.Username, this.Password),
                    "Próba logowania zakończyła się niepowodzeniem. Sprawdź dane logowania i spróbuj ponownie."
                );

                if (success)
                {
                    var appShell = IPlatformApplication.Current.Services.GetService<AppShell>();
                    if (appShell != null)
                    {
                        Application.Current.MainPage = appShell;
                    }
                    else
                    {
                        Application.Current.MainPage = new AppShell(this.authService);
                    }
                }
                else
                {
                    // This will be handled by the ExceptionHandler if login fails with an exception,
                    // but we handle the false result explicitly
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd logowania",
                        "Nieprawidłowa nazwa użytkownika lub hasło.",
                        "OK");
                }
            }
            catch (InvalidInputException)
            {
                // This will be caught and handled by the SafeExecute method
                // No need to handle here unless you want custom behavior
                throw;
            }
            catch (Exception ex)
            {
                // Fallback in case ExceptionHandler fails
                await this.dialogService.DisplayAlertAsync(
                    "Błąd logowania",
                    "Wystąpił nieoczekiwany błąd podczas logowania.",
                    "OK");
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnGoToRegisterAsync()
        {
            if (this.IsBusy)
                return;

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                if (Application.Current.MainPage is NavigationPage navigationPage)
                {
                    var registerViewModel = IPlatformApplication.Current.Services.GetRequiredService<RegisterViewModel>();
                    var registerPage = new Views.Authentication.RegisterPage(registerViewModel);
                    await navigationPage.PushAsync(registerPage);
                }
            }, "Nie można przejść do ekranu rejestracji.");

            this.IsBusy = false;
        }
    }
}
