using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
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

        public RegisterViewModel(
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;
            this.Title = "Rejestracja";
            this.AvailableSpecializations = new ObservableCollection<SpecializationProgram>();
            this.RegisterCommand = new AsyncRelayCommand(this.OnRegisterAsync, this.CanRegister);
            this.GoToLoginCommand = new AsyncRelayCommand(OnGoToLoginAsync);
        }

        public async Task InitializeAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                await this.LoadSpecializationsAsync();
            }, "Nie udało się załadować dostępnych specjalizacji.");

            this.IsBusy = false;
        }

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

        public ICommand RegisterCommand { get; }

        public ICommand GoToLoginCommand { get; }

        private async Task LoadSpecializationsAsync()
        {
            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                SmkVersion currentVersion = this.IsNewSmkVersion ? SmkVersion.New : SmkVersion.Old;
                var programs = await SpecializationLoader.LoadAllSpecializationProgramsForVersionAsync(currentVersion);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.AvailableSpecializations.Clear();
                    foreach (var program in programs)
                    {
                        this.AvailableSpecializations.Add(program);
                    }

                    if (this.AvailableSpecializations.Count > 0)
                    {
                        this.SelectedSpecialization = this.AvailableSpecializations[0];
                    }
                    else
                    {
                        this.SelectedSpecialization = null;
                    }
                });
            }, "Nie udało się załadować dostępnych specjalizacji.");

            this.IsBusy = false;
        }

        private void ValidatePasswords()
        {
            bool passwordsMatch = !string.IsNullOrEmpty(this.Password) &&
                                 !string.IsNullOrEmpty(this.ConfirmPassword) &&
                                 this.Password == this.ConfirmPassword;
            this.PasswordsNotMatch = !passwordsMatch;
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
                // Input validation with domain exceptions
                if (string.IsNullOrWhiteSpace(this.Username))
                {
                    throw new InvalidInputException(
                        "Username is required",
                        "Nazwa użytkownika jest wymagana.");
                }

                if (string.IsNullOrWhiteSpace(this.Name))
                {
                    throw new InvalidInputException(
                        "Name is required",
                        "Imię i nazwisko jest wymagane.");
                }

                if (string.IsNullOrWhiteSpace(this.Password))
                {
                    throw new InvalidInputException(
                        "Password is required",
                        "Hasło jest wymagane.");
                }

                if (string.IsNullOrWhiteSpace(this.Email))
                {
                    throw new InvalidInputException(
                        "Email is required",
                        "Adres email jest wymagany.");
                }

                if (this.PasswordsNotMatch)
                {
                    throw new InvalidInputException(
                        "Passwords do not match",
                        "Hasła nie są identyczne.");
                }

                if (this.SelectedSpecialization == null)
                {
                    throw new InvalidInputException(
                        "No specialization selected",
                        "Nie wybrano specjalizacji. Wybierz specjalizację przed rejestracją.");
                }

                await SafeExecuteAsync(async () =>
                {
                    var user = new User
                    {
                        Username = this.Username,
                        Name = this.Name,
                        Email = this.Email,
                        RegistrationDate = DateTime.Now,
                        SmkVersion = this.IsNewSmkVersion ? SmkVersion.New : SmkVersion.Old,
                    };

                    if (string.IsNullOrEmpty(this.SelectedSpecialization.Structure))
                    {
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

                    var specialization = new Models.Specialization
                    {
                        Name = this.SelectedSpecialization.Name,
                        ProgramCode = this.SelectedSpecialization.Code,
                        SmkVersion = user.SmkVersion,
                        StartDate = DateTime.Now,
                        PlannedEndDate = DateTime.Now.AddMonths(
                            this.SelectedSpecialization.TotalDuration != null && this.SelectedSpecialization.TotalDuration.TotalMonths > 0
                                ? this.SelectedSpecialization.TotalDuration.TotalMonths
                                : 60),
                        ProgramStructure = this.SelectedSpecialization.Structure,
                        DurationYears = this.SelectedSpecialization.DurationYears,
                    };

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
                        specialization.Modules = new List<Models.Module>();
                    }

                    specialization.CalculatedEndDate = specialization.PlannedEndDate;
                    bool success = await this.authService.RegisterAsync(user, this.Password, specialization);

                    if (success)
                    {
                        await this.specializationService.InitializeSpecializationModulesAsync(specialization.SpecializationId);
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            "Rejestracja zakończona pomyślnie. Zaloguj się, aby rozpocząć korzystanie z aplikacji.",
                            "OK");

                        await OnGoToLoginAsync();
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Registration failed",
                            "Nie udało się zarejestrować. Sprawdź podane dane i spróbuj ponownie.");
                    }
                }, "Wystąpił problem podczas rejestracji. Sprawdź podane dane i spróbuj ponownie.");
            }
            catch (InvalidInputException)
            {
                // These will be handled by the SafeExecuteAsync method
                throw;
            }
            catch (Exception ex)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił nieoczekiwany błąd podczas rejestracji.",
                    "OK");
                System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private static async Task OnGoToLoginAsync()
        {
            if (Application.Current?.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.PopAsync();
            }
            else
            {
                var loginViewModel = IPlatformApplication.Current.Services.GetService<SledzSpecke.App.ViewModels.Authentication.LoginViewModel>();
                if (loginViewModel != null)
                {
                    var loginPage = new SledzSpecke.App.Views.Authentication.LoginPage(loginViewModel);
                    Application.Current.MainPage = new NavigationPage(loginPage);
                }
            }
        }
    }
}
