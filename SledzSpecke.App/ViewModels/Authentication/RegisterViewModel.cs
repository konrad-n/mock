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
                System.Diagnostics.Debug.WriteLine("Inicjalizacja RegisterViewModel...");

                // Załadowanie dostępnych specjalizacji dla domyślnie wybranej wersji (Nowej)
                await this.LoadSpecializationsAsync();

                System.Diagnostics.Debug.WriteLine($"Inicjalizacja zakończona. Załadowano {this.AvailableSpecializations.Count} specjalizacji.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji RegisterViewModel: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

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
                System.Diagnostics.Debug.WriteLine("Rozpoczynam ładowanie specjalizacji...");

                // Wybierz aktualną wersję SMK na podstawie zaznaczenia checkbox'ów
                SmkVersion currentVersion = this.IsNewSmkVersion ? SmkVersion.New : SmkVersion.Old;
                System.Diagnostics.Debug.WriteLine($"Wybrana wersja SMK: {currentVersion}");

                // Użyj ulepszonego SpecializationLoader do załadowania programów specjalizacji
                var programs = await SpecializationLoader.LoadAllSpecializationProgramsForVersionAsync(currentVersion);
                System.Diagnostics.Debug.WriteLine($"Załadowano {programs.Count} programów specjalizacji");

                // Zaktualizuj kolekcję w bezpieczny sposób na głównym wątku UI
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.AvailableSpecializations.Clear();
                    foreach (var program in programs)
                    {
                        this.AvailableSpecializations.Add(program);
                        System.Diagnostics.Debug.WriteLine($"Dodano specjalizację: {program.Name} ({program.SmkVersion})");
                    }

                    // Wybierz pierwszą dostępną specjalizację jeśli istnieje
                    if (this.AvailableSpecializations.Count > 0)
                    {
                        this.SelectedSpecialization = this.AvailableSpecializations[0];
                        System.Diagnostics.Debug.WriteLine($"Wybrano specjalizację: {this.SelectedSpecialization.Name}");
                    }
                    else
                    {
                        this.SelectedSpecialization = null;
                        System.Diagnostics.Debug.WriteLine("Brak dostępnych specjalizacji!");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania specjalizacji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

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
            bool isPasswordValid = !string.IsNullOrWhiteSpace(this.Password);
            bool isConfirmPasswordValid = !string.IsNullOrWhiteSpace(this.ConfirmPassword);
            bool isEmailValid = !string.IsNullOrWhiteSpace(this.Email);
            bool arePasswordsMatching = !this.PasswordsNotMatch;
            bool isSpecializationSelected = this.SelectedSpecialization != null;
            bool isSmkVersionSelected = this.IsOldSmkVersion || this.IsNewSmkVersion;

            return isUsernameValid && isPasswordValid && isConfirmPasswordValid &&
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

                System.Diagnostics.Debug.WriteLine($"Wybrana specjalizacja: {this.SelectedSpecialization.Name}");
                System.Diagnostics.Debug.WriteLine($"Wersja SMK: {(this.IsNewSmkVersion ? "Nowa" : "Stara")}");

                // Utworzenie obiektu użytkownika
                var user = new User
                {
                    Username = this.Username,
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
                        System.Diagnostics.Debug.WriteLine("Pomyślnie załadowano strukturę specjalizacji");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Nie udało się załadować struktury, używam domyślnej");
                        this.SelectedSpecialization.Structure = $"{{ \"name\": \"{this.SelectedSpecialization.Name}\", \"code\": \"{this.SelectedSpecialization.Code}\" }}";
                    }
                }

                // Utworzenie specjalizacji
                var specialization = new Models.Specialization
                {
                    Name = this.SelectedSpecialization.Name,
                    ProgramCode = this.SelectedSpecialization.Code,
                    StartDate = DateTime.Now,
                    PlannedEndDate = DateTime.Now.AddMonths(this.SelectedSpecialization.TotalDurationMonths > 0
                        ? this.SelectedSpecialization.TotalDurationMonths
                        : 60),
                    ProgramStructure = this.SelectedSpecialization.Structure,
                    HasModules = true,  // Wszystkie specjalizacje mają moduły
                };

                // Tworzenie modułów dla specjalizacji
                System.Diagnostics.Debug.WriteLine("Tworzenie modułów dla specjalizacji z użyciem rzeczywistych danych...");

                // Użycie ulepszonej metody async
                var modules = await ModuleHelper.CreateModulesForSpecializationAsync(
                    specialization.ProgramCode,
                    specialization.StartDate,
                    user.SmkVersion);

                if (modules != null && modules.Count > 0)
                {
                    specialization.Modules = modules;
                    System.Diagnostics.Debug.WriteLine($"Utworzono {modules.Count} modułów z rzeczywistymi danymi");
                }
                else
                {
                    // Ten przypadek nie powinien wystąpić, ale zostawiamy jako zabezpieczenie
                    System.Diagnostics.Debug.WriteLine("Błąd: Nie utworzono żadnych modułów! Używam pustej listy jako ostateczność.");
                    specialization.Modules = new List<Models.Module>();
                }

                // Obliczenie daty zakończenia
                specialization.CalculatedEndDate = specialization.PlannedEndDate;

                // Rejestracja użytkownika
                bool success = await this.authService.RegisterAsync(user, this.Password, specialization);

                if (success)
                {
                    System.Diagnostics.Debug.WriteLine("Rejestracja zakończona sukcesem!");

                    try
                    {
                        // Inicjalizacja modułów po rejestracji
                        await this.specializationService.InitializeSpecializationModulesAsync(specialization.SpecializationId);
                        System.Diagnostics.Debug.WriteLine("Inicjalizacja modułów po rejestracji zakończona");
                    }
                    catch (Exception initEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd inicjalizacji modułów po rejestracji: {initEx.Message}");
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
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas wyświetlania alertu: {alertEx.Message}");
                        // Kontynuuj mimo błędu alertu
                    }

                    // Bezpieczne przejście do ekranu logowania
                    await this.OnGoToLoginAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Rejestracja nie powiodła się");

                    try
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się zarejestrować. Sprawdź podane dane i spróbuj ponownie.",
                            "OK");
                    }
                    catch (Exception alertEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas wyświetlania alertu: {alertEx.Message}");
                        // Kontynuuj mimo błędu alertu
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas rejestracji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

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
                System.Diagnostics.Debug.WriteLine("Przechodzę do ekranu logowania...");

                // Sprawdź, czy MainPage to NavigationPage
                if (Application.Current?.MainPage is NavigationPage navigationPage)
                {
                    await navigationPage.PopAsync();
                    System.Diagnostics.Debug.WriteLine("Powrót do poprzedniej strony przez PopAsync");
                }
                else
                {
                    // Jeśli nie jesteśmy w NavigationPage, spróbuj ustawić MainPage
                    var loginViewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Authentication.LoginViewModel>();
                    if (loginViewModel != null)
                    {
                        var loginPage = new SledzSpecke.App.Views.Authentication.LoginPage(loginViewModel);
                        Application.Current.MainPage = new NavigationPage(loginPage);
                        System.Diagnostics.Debug.WriteLine("Ustawiono nową stronę logowania");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Nie udało się uzyskać LoginViewModel z DI");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas przechodzenia do ekranu logowania: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}