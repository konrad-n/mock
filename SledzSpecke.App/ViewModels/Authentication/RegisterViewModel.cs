using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Authentication
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private string username;
        private string name;
        private string password;
        private string confirmPassword;
        private string email;
        private ObservableCollection<SpecializationProgram> availableSpecializations;
        private SpecializationProgram selectedSpecialization;
        private bool isOldSmkVersion;
        private bool isNewSmkVersion = true;
        private bool passwordsNotMatch;

        public RegisterViewModel(IAuthService authService, IDialogService dialogService, ISpecializationService specializationService)
        {
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Rejestracja";
            this.AvailableSpecializations = new ObservableCollection<SpecializationProgram>();

            // Inicjalizacja komend
            this.RegisterCommand = new AsyncRelayCommand(this.OnRegisterAsync, this.CanRegister);
            this.GoToLoginCommand = new AsyncRelayCommand(this.OnGoToLoginAsync);
        }

        // Metoda inicjalizacji, którą należy wywołać przed wyświetleniem widoku
        public async Task InitializeAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Załadowanie dostępnych specjalizacji dla domyślnie wybranej wersji (Nowej)
                await this.LoadSpecializationsAsync();

            }
            catch (Exception ex)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować listy specjalizacji. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        // Właściwości do bindowania
        public string Username
        {
            get => this.username;
            set
            {
                if (this.SetProperty(ref this.username, value))
                {
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string Name
        {
            get => this.name;
            set
            {
                if (this.SetProperty(ref this.name, value))
                {
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
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
                    this.ValidatePasswords();
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string ConfirmPassword
        {
            get => this.confirmPassword;
            set
            {
                if (this.SetProperty(ref this.confirmPassword, value))
                {
                    this.ValidatePasswords();
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string Email
        {
            get => this.email;
            set
            {
                if (this.SetProperty(ref this.email, value))
                {
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<SpecializationProgram> AvailableSpecializations
        {
            get => this.availableSpecializations;
            set => this.SetProperty(ref this.availableSpecializations, value);
        }

        public SpecializationProgram SelectedSpecialization
        {
            get => this.selectedSpecialization;
            set
            {
                if (this.SetProperty(ref this.selectedSpecialization, value))
                {
                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsNewSmkVersion
        {
            get => this.isNewSmkVersion;
            set
            {
                if (this.SetProperty(ref this.isNewSmkVersion, value) && value)
                {
                    this.IsOldSmkVersion = false;

                    // Przeładowanie specjalizacji dla nowej wersji SMK
                    this.LoadSpecializationsAsync().ConfigureAwait(false);

                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set
            {
                if (this.SetProperty(ref this.isOldSmkVersion, value) && value)
                {
                    this.IsNewSmkVersion = false;

                    // Przeładowanie specjalizacji dla starej wersji SMK
                    this.LoadSpecializationsAsync().ConfigureAwait(false);

                    ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public bool PasswordsNotMatch
        {
            get => this.passwordsNotMatch;
            set => this.SetProperty(ref this.passwordsNotMatch, value);
        }

        // Komendy
        public ICommand RegisterCommand { get; }

        public ICommand GoToLoginCommand { get; }

        // Metody pomocnicze
        private async Task LoadSpecializationsAsync()
        {
            try
            {
                this.IsBusy = true;

                // Wybierz aktualną wersję SMK na podstawie zaznaczenia checkbox'ów
                SmkVersion currentVersion = this.IsNewSmkVersion ? SmkVersion.New : SmkVersion.Old;

                // Użyj ulepszonego SpecializationLoader do załadowania programów specjalizacji
                var programs = await SpecializationLoader.LoadAllSpecializationProgramsForVersionAsync(currentVersion);

                // Zaktualizuj kolekcję w bezpieczny sposób na głównym wątku UI
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.AvailableSpecializations.Clear();
                    foreach (var program in programs)
                    {
                        this.AvailableSpecializations.Add(program);
                    }

                    // Wybierz pierwszą dostępną specjalizację jeśli istnieje
                    if (this.AvailableSpecializations.Count > 0)
                    {
                        this.SelectedSpecialization = this.AvailableSpecializations[0];
                    }
                    else
                    {
                        this.SelectedSpecialization = null;
                    }
                });
            }
            catch (Exception ex)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować listy specjalizacji. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void ValidatePasswords()
        {
            bool passwordsMatch = !string.IsNullOrEmpty(this.Password) &&
                                 !string.IsNullOrEmpty(this.ConfirmPassword) &&
                                 this.Password == this.ConfirmPassword;

            this.PasswordsNotMatch = !passwordsMatch;

            System.Diagnostics.Debug.WriteLine($"Walidacja haseł: Zgodne={passwordsMatch}, NieZgodne={this.PasswordsNotMatch}");

            ((AsyncRelayCommand)this.RegisterCommand).NotifyCanExecuteChanged();
        }

        private bool CanRegister()
        {
            bool isUsernameValid = !string.IsNullOrWhiteSpace(this.Username);
            bool isNameValid = !string.IsNullOrWhiteSpace(this.Name);
            bool isPasswordValid = !string.IsNullOrWhiteSpace(this.Password);
            bool isConfirmPasswordValid = !string.IsNullOrWhiteSpace(this.ConfirmPassword);
            bool isEmailValid = !string.IsNullOrWhiteSpace(this.Email);
            bool arePasswordsMatching = !this.PasswordsNotMatch;
            bool isSpecializationSelected = this.SelectedSpecialization != null;
            bool isSmkVersionSelected = this.IsOldSmkVersion || this.IsNewSmkVersion;

            return isUsernameValid && isNameValid && isPasswordValid && isConfirmPasswordValid &&
                   isEmailValid && arePasswordsMatching && isSpecializationSelected &&
                   isSmkVersionSelected;
        }

        private async Task OnRegisterAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                System.Diagnostics.Debug.WriteLine("Rozpoczynam proces rejestracji...");

                // Weryfikacja wybranej specjalizacji
                if (this.SelectedSpecialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie wybrano specjalizacji. Wybierz specjalizację przed rejestracją.",
                        "OK");
                    return;
                }

                // Utworzenie obiektu użytkownika
                var user = new User
                {
                    Username = this.Username,
                    Name = this.Name,
                    Email = this.Email,
                    RegistrationDate = DateTime.Now,
                    SmkVersion = this.IsNewSmkVersion ? SmkVersion.New : SmkVersion.Old,
                };

                // Sprawdzenie czy mamy strukturę specjalizacji
                if (string.IsNullOrEmpty(this.SelectedSpecialization.Structure))
                {
                    System.Diagnostics.Debug.WriteLine("Struktura specjalizacji jest pusta, próbuję załadować...");

                    // Ładowanie pełnej struktury specjalizacji
                    var fullSpecialization = await SpecializationLoader.LoadSpecializationProgramAsync(
                        this.SelectedSpecialization.Code,
                        user.SmkVersion);

                    if (fullSpecialization != null && !string.IsNullOrEmpty(fullSpecialization.Structure))
                    {
                        this.SelectedSpecialization.Structure = fullSpecialization.Structure;
                    }
                    else
                    {
                        this.SelectedSpecialization.Structure = $"{{ \"name\": \"{this.SelectedSpecialization.Name}\", \"code\": \"{this.SelectedSpecialization.Code}\" }}";
                    }
                }

                // Utworzenie specjalizacji
                var specialization = new Models.Specialization
                {
                    Name = this.SelectedSpecialization.Name,
                    ProgramCode = this.SelectedSpecialization.Code,
                    SmkVersion = user.SmkVersion,
                    StartDate = DateTime.Now,
                    PlannedEndDate = DateTime.Now.AddMonths(
                        this.SelectedSpecialization.TotalDuration != null && this.SelectedSpecialization.TotalDuration.TotalMonths > 0
                            ? this.SelectedSpecialization.TotalDuration.TotalMonths
                            : 60), // Domyślnie 5 lat (60 miesięcy)
                    ProgramStructure = this.SelectedSpecialization.Structure,
                    DurationYears = this.SelectedSpecialization.DurationYears,
                };

                // Użycie ulepszonej metody async
                var modules = await ModuleHelper.CreateModulesForSpecializationAsync(
                    specialization.ProgramCode,
                    specialization.StartDate,
                    user.SmkVersion,
                    specialization.SpecializationId);

                if (modules != null && modules.Count > 0)
                {
                    specialization.Modules = modules;
                }
                else
                {
                    // Ten przypadek nie powinien wystąpić, ale zostawiamy jako zabezpieczenie
                    specialization.Modules = new List<Models.Module>();
                }

                // Obliczenie daty zakończenia
                specialization.CalculatedEndDate = specialization.PlannedEndDate;

                // Rejestracja użytkownika
                bool success = await this.authService.RegisterAsync(user, this.Password, specialization);

                if (success)
                {
                    try
                    {
                        // Inicjalizacja modułów po rejestracji
                        await this.specializationService.InitializeSpecializationModulesAsync(specialization.SpecializationId);
                    }
                    catch (Exception initEx)
                    {
                        // Kontynuuj mimo błędu inicjalizacji
                    }

                    try
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            "Rejestracja zakończona pomyślnie. Zaloguj się, aby rozpocząć korzystanie z aplikacji.",
                            "OK");
                    }
                    catch (Exception alertEx)
                    {
                        // Kontynuuj mimo błędu alertu
                    }

                    // Bezpieczne przejście do ekranu logowania
                    await this.OnGoToLoginAsync();
                }
                else
                {
                    try
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się zarejestrować. Sprawdź podane dane i spróbuj ponownie.",
                            "OK");
                    }
                    catch (Exception alertEx)
                    {
                        // Kontynuuj mimo błędu alertu
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        $"Wystąpił błąd podczas rejestracji: {ex.Message}",
                        "OK");
                }
                catch
                {
                    // Ignoruj błędy alertów w przypadku wyjątku
                }
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnGoToLoginAsync()
        {
            try
            {
                // Sprawdź, czy MainPage to NavigationPage
                if (Application.Current?.MainPage is NavigationPage navigationPage)
                {
                    await navigationPage.PopAsync();
                }
                else
                {
                    // Jeśli nie jesteśmy w NavigationPage, spróbuj ustawić MainPage
                    var loginViewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Authentication.LoginViewModel>();
                    if (loginViewModel != null)
                    {
                        var loginPage = new SledzSpecke.App.Views.Authentication.LoginPage(loginViewModel);
                        Application.Current.MainPage = new NavigationPage(loginPage);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}