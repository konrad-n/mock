using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Authentication
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;

        private string username;
        private string password;
        private string confirmPassword;
        private string email;
        private ObservableCollection<SpecializationProgram> availableSpecializations;
        private SpecializationProgram selectedSpecialization;
        private bool isOldSmkVersion;
        private bool isNewSmkVersion;
        private bool passwordsNotMatch;

        public RegisterViewModel(IAuthService authService, IDialogService dialogService)
        {
            this.authService = authService;
            this.dialogService = dialogService;

            this.Title = "Rejestracja";
            this.AvailableSpecializations = new ObservableCollection<SpecializationProgram>();
            this.IsNewSmkVersion = true; // Domyślnie nowa wersja SMK
            this.PasswordsNotMatch = true;

            // Inicjalizacja komend
            this.RegisterCommand = new AsyncRelayCommand(this.OnRegisterAsync, this.CanRegister);
            this.GoToLoginCommand = new AsyncRelayCommand(this.OnGoToLoginAsync);

            // Nie ładujemy specjalizacji w konstruktorze, zrobimy to w InitializeAsync
        }

        public bool IsInitialized { get; private set; }

        public bool IsLoading
        {
            get => this.IsBusy;
            set => this.SetProperty(ref this.isBusy, value);
        }

        // Metoda inicjalizacji, którą należy wywołać przed wyświetleniem widoku
        public async Task InitializeAsync()
        {
            if (this.IsInitialized)
            {
                return;
            }

            this.IsLoading = true;

            try
            {
                // Załadowanie dostępnych specjalizacji
                await this.LoadSpecializationsAsync();
                this.IsInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji RegisterViewModel: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować listy specjalizacji. Sprawdź połączenie z internetem i spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsLoading = false;
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

        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set
            {
                if (this.SetProperty(ref this.isOldSmkVersion, value) && value)
                {
                    this.IsNewSmkVersion = false;
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

                // Sprawdzenie czy katalog istnieje
                string templatesDirPath = Constants.SpecializationTemplatesPath;
                if (!Directory.Exists(templatesDirPath))
                {
                    Directory.CreateDirectory(templatesDirPath);
                }

                // Próba załadowania szablonów z zasobów
                await this.LoadSpecializationTemplatesFromResourcesAsync();

                // Jeśli nie ma dostępnych specjalizacji, spróbuj załadować z zasobów
                if (this.AvailableSpecializations.Count == 0)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono dostępnych specjalizacji. Sprawdź zasoby aplikacji.",
                        "OK");
                }
                else
                {
                    // Wybierz pierwszą specjalizację jako domyślną
                    this.SelectedSpecialization = this.AvailableSpecializations[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania specjalizacji: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować listy specjalizacji.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadSpecializationTemplatesFromResourcesAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Rozpoczynam ładowanie szablonów specjalizacji z zasobów...");

                // Załadowanie szablonów specjalizacji z zasobów aplikacji
                var assembly = typeof(RegisterViewModel).Assembly;
                var resourcePrefix = "SledzSpecke.App.Resources.Raw.SpecializationTemplates.";

                var resourceNames = assembly.GetManifestResourceNames();
                System.Diagnostics.Debug.WriteLine($"Znalezione zasoby: {string.Join(", ", resourceNames)}");

                var matchingResources = resourceNames
                    .Where(r => r.StartsWith(resourcePrefix) && r.EndsWith(".json"))
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Znaleziono {matchingResources.Count} pasujących zasobów.");

                // Jeśli nie znaleziono żadnych zasobów, spróbuj załadować z katalogu specjalizacji
                if (matchingResources.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Nie znaleziono zasobów w pakiecie, próbuję załadować z katalogu specjalizacji...");
                    await this.LoadSpecializationTemplatesFromDirectoryAsync();
                    return;
                }

                this.AvailableSpecializations.Clear();

                // Dodaj domyślne specjalizacje, jeśli nie ma żadnych
                if (matchingResources.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Dodaję domyślne specjalizacje...");
                    this.AddDefaultSpecializations();
                    return;
                }

                foreach (var resourceName in matchingResources)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Ładowanie zasobu: {resourceName}");
                        using (var stream = assembly.GetManifestResourceStream(resourceName))
                        {
                            if (stream != null)
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    string json = await reader.ReadToEndAsync();

                                    if (string.IsNullOrEmpty(json))
                                    {
                                        System.Diagnostics.Debug.WriteLine("Plik JSON jest pusty!");
                                        continue;
                                    }

                                    try
                                    {
                                        var program = JsonSerializer.Deserialize<SpecializationProgram>(json);

                                        if (program != null)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Pomyślnie zdeserializowano program: {program.Name}");

                                            // Dodanie do kolekcji
                                            this.AvailableSpecializations.Add(program);

                                            // Zapisanie do pliku w katalogu aplikacji
                                            string fileName = Path.GetFileName(resourceName);
                                            if (string.IsNullOrEmpty(fileName))
                                            {
                                                fileName = $"{program.Code.ToLower()}.json";
                                            }

                                            string filePath = Path.Combine(Constants.SpecializationTemplatesPath, fileName);

                                            if (!File.Exists(filePath))
                                            {
                                                await File.WriteAllTextAsync(filePath, json);
                                                System.Diagnostics.Debug.WriteLine($"Zapisano plik: {filePath}");
                                            }
                                        }
                                        else
                                        {
                                            System.Diagnostics.Debug.WriteLine("Deserializacja zwróciła null!");
                                        }
                                    }
                                    catch (JsonException jsonEx)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Błąd deserializacji JSON: {jsonEx.Message}");
                                    }
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Nie można otworzyć strumienia dla: {resourceName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd ładowania szablonu {resourceName}: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Załadowano {this.AvailableSpecializations.Count} specjalizacji.");

                // Jeśli nadal nie ma żadnych specjalizacji, dodaj domyślne
                if (this.AvailableSpecializations.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Nie udało się załadować żadnych specjalizacji, dodaję domyślne...");
                    this.AddDefaultSpecializations();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania szablonów specjalizacji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                // Dodaj domyślne specjalizacje w przypadku błędu
                this.AddDefaultSpecializations();
            }
        }

        private async Task LoadSpecializationTemplatesFromDirectoryAsync()
        {
            try
            {
                string templatesDirPath = Constants.SpecializationTemplatesPath;
                if (!Directory.Exists(templatesDirPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Katalog {templatesDirPath} nie istnieje.");
                    return;
                }

                var files = Directory.GetFiles(templatesDirPath, "*.json");
                System.Diagnostics.Debug.WriteLine($"Znaleziono {files.Length} plików JSON w katalogu.");

                foreach (var file in files)
                {
                    try
                    {
                        string json = await File.ReadAllTextAsync(file);
                        var program = JsonSerializer.Deserialize<SpecializationProgram>(json);

                        if (program != null)
                        {
                            this.AvailableSpecializations.Add(program);
                            System.Diagnostics.Debug.WriteLine($"Załadowano specjalizację: {program.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd ładowania pliku {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania z katalogu: {ex.Message}");
            }
        }

        private void AddDefaultSpecializations()
        {
            // Dodaj kilka podstawowych specjalizacji jako awaryjne rozwiązanie
            this.AvailableSpecializations.Add(new SpecializationProgram
            {
                ProgramId = 1,
                Name = "Choroby wewnętrzne",
                Code = "internal_medicine",
                SmkVersion = SmkVersion.New,
                HasModules = false,
                TotalDurationMonths = 48
            });

            this.AvailableSpecializations.Add(new SpecializationProgram
            {
                ProgramId = 2,
                Name = "Kardiologia",
                Code = "cardiology",
                SmkVersion = SmkVersion.New,
                HasModules = true,
                BasicModuleCode = "internal_medicine",
                BasicModuleDurationMonths = 24,
                TotalDurationMonths = 60
            });

            this.AvailableSpecializations.Add(new SpecializationProgram
            {
                ProgramId = 3,
                Name = "Psychiatria",
                Code = "psychiatry",
                SmkVersion = SmkVersion.New,
                HasModules = false,
                TotalDurationMonths = 48
            });

            System.Diagnostics.Debug.WriteLine("Dodano domyślne specjalizacje.");
        }

        private void ValidatePasswords()
        {
            this.PasswordsNotMatch = string.IsNullOrEmpty(this.Password) ||
                                 string.IsNullOrEmpty(this.ConfirmPassword) ||
                                 this.Password != this.ConfirmPassword;
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(this.Username) &&
                   !string.IsNullOrWhiteSpace(this.Password) &&
                   !string.IsNullOrWhiteSpace(this.ConfirmPassword) &&
                   !string.IsNullOrWhiteSpace(this.Email) &&
                   this.IsPasswordValid(this.Password) &&
                   this.PasswordsNotMatch &&
                   this.SelectedSpecialization != null &&
                   (this.IsOldSmkVersion || this.IsNewSmkVersion);
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
                // Weryfikacja wybranej specjalizacji
                if (this.SelectedSpecialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie wybrano specjalizacji. Wybierz specjalizację przed rejestracją.",
                        "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("Rozpoczynam proces rejestracji...");
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

                // Przygotowanie struktury specjalizacji
                System.Diagnostics.Debug.WriteLine("Ładowanie struktury specjalizacji...");
                string specializationStructure = null;

                try
                {
                    specializationStructure = await this.LoadSpecializationStructureAsync(
                        this.SelectedSpecialization.Code,
                        user.SmkVersion);

                    if (string.IsNullOrEmpty(specializationStructure))
                    {
                        throw new Exception("Nie udało się załadować struktury specjalizacji - zawartość jest pusta");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd ładowania struktury specjalizacji: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        $"Nie udało się załadować struktury specjalizacji. Szczegóły: {ex.Message}",
                        "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("Struktura specjalizacji załadowana pomyślnie.");

                // Utworzenie specjalizacji z wybranego szablonu
                var specialization = new Models.Specialization
                {
                    Name = this.SelectedSpecialization.Name,
                    ProgramCode = this.SelectedSpecialization.Code,
                    StartDate = DateTime.Now,
                    PlannedEndDate = DateTime.Now.AddMonths(this.SelectedSpecialization.TotalDurationMonths > 0 ? this.SelectedSpecialization.TotalDurationMonths : 36),
                    ProgramStructure = specializationStructure,
                    HasModules = this.SelectedSpecialization.HasModules,
                };

                // Jeśli specjalizacja posiada moduły, utwórz je
                if (specialization.HasModules)
                {
                    System.Diagnostics.Debug.WriteLine("Tworzenie modułów dla specjalizacji...");
                    var modules = ModuleHelper.CreateModulesForSpecialization(
                        specialization.ProgramCode,
                        specialization.StartDate);

                    if (modules != null && modules.Count > 0)
                    {
                        specialization.Modules = modules;
                        System.Diagnostics.Debug.WriteLine($"Utworzono {modules.Count} modułów.");
                    }
                    else
                    {
                        specialization.Modules = new List<Models.Module>();
                        System.Diagnostics.Debug.WriteLine("Funkcja CreateModulesForSpecialization zwróciła pustą listę modułów.");
                    }
                }

                // Obliczenie daty zakończenia na podstawie programu
                specialization.CalculatedEndDate = specialization.PlannedEndDate;

                // Rejestracja użytkownika
                System.Diagnostics.Debug.WriteLine("Rozpoczynam rejestrację użytkownika...");
                bool success = await this.authService.RegisterAsync(user, this.Password, specialization);

                if (success)
                {
                    System.Diagnostics.Debug.WriteLine("Rejestracja zakończona sukcesem!");
                    // Powiadomienie o sukcesie
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        "Rejestracja zakończona pomyślnie. Zaloguj się, aby rozpocząć korzystanie z aplikacji.",
                        "OK");

                    await this.OnGoToLoginAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Rejestracja nie powiodła się.");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zarejestrować. Sprawdź podane dane i spróbuj ponownie.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas rejestracji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Wystąpił błąd podczas rejestracji: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task<string> LoadSpecializationStructureAsync(string programCode, SmkVersion smkVersion)
        {
            System.Diagnostics.Debug.WriteLine($"Ładowanie struktury dla specjalizacji: {programCode}, wersja SMK: {smkVersion}");

            try
            {
                // Sprawdzenie i utworzenie katalogu specjalizacji jeśli nie istnieje
                string templatesDirPath = Constants.SpecializationTemplatesPath;
                if (!Directory.Exists(templatesDirPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Tworzenie katalogu specjalizacji: {templatesDirPath}");
                    Directory.CreateDirectory(templatesDirPath);
                }

                // Ścieżka do pliku specjalizacji
                string filePath = Path.Combine(templatesDirPath, $"{programCode.ToLower()}.json");
                System.Diagnostics.Debug.WriteLine($"Ścieżka do pliku specjalizacji: {filePath}");

                // Sprawdzenie czy plik istnieje
                if (File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine("Plik specjalizacji istnieje na dysku, odczytuję...");
                    string json = await File.ReadAllTextAsync(filePath);
                    if (string.IsNullOrEmpty(json))
                    {
                        System.Diagnostics.Debug.WriteLine("Odczytany plik jest pusty!");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Odczytano {json.Length} znaków z pliku.");
                    }
                    return json;
                }

                System.Diagnostics.Debug.WriteLine("Plik nie istnieje na dysku, próbuję odczytać z zasobów...");

                // Próba ładowania z zasobów
                var assembly = typeof(RegisterViewModel).Assembly;
                string resourceName = $"SledzSpecke.App.Resources.Raw.SpecializationTemplates.{programCode.ToLower()}.json";

                System.Diagnostics.Debug.WriteLine($"Szukam zasobu: {resourceName}");

                // Sprawdzenie czy zasób istnieje
                var resourceNames = assembly.GetManifestResourceNames();
                System.Diagnostics.Debug.WriteLine($"Dostępne zasoby: {string.Join(", ", resourceNames)}");

                if (!resourceNames.Contains(resourceName))
                {
                    System.Diagnostics.Debug.WriteLine($"Zasób {resourceName} nie istnieje. Próbuję znaleźć podobny...");

                    // Szukanie podobnego zasobu
                    var matchingResource = resourceNames
                        .FirstOrDefault(r => r.EndsWith($".{programCode.ToLower()}.json") ||
                                           r.Contains($".{programCode.ToLower()}."));

                    if (!string.IsNullOrEmpty(matchingResource))
                    {
                        System.Diagnostics.Debug.WriteLine($"Znaleziono podobny zasób: {matchingResource}");
                        resourceName = matchingResource;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Nie znaleziono pasującego zasobu!");

                        // Wypróbuj domyślny zasób
                        resourceName = "SledzSpecke.App.Resources.Raw.SpecializationTemplates.internal_medicine.json";
                        System.Diagnostics.Debug.WriteLine($"Próbuję użyć domyślnego zasobu: {resourceName}");
                    }
                }

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Udało się otworzyć strumień zasobu.");
                        using (var reader = new StreamReader(stream))
                        {
                            string json = await reader.ReadToEndAsync();
                            System.Diagnostics.Debug.WriteLine($"Odczytano {json.Length} znaków z zasobu.");

                            // Zapisz do pliku na dysku do przyszłego użycia
                            try
                            {
                                await File.WriteAllTextAsync(filePath, json);
                                System.Diagnostics.Debug.WriteLine($"Zapisano zasób do pliku: {filePath}");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania do pliku: {ex.Message}");
                            }

                            return json;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Nie można otworzyć strumienia zasobu dla: {resourceName}");
                    }
                }

                throw new FileNotFoundException($"Nie znaleziono pliku specjalizacji dla kodu {programCode}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania struktury specjalizacji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task OnGoToLoginAsync()
        {
            // Powrót do strony logowania
            if (Application.Current.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.PopAsync();
            }
        }

        // Walidacja poprawności hasła (min. 6 znaków, zawiera cyfry i litery)
        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                return false;
            }

            bool hasDigit = password.Any(char.IsDigit);
            bool hasLetter = password.Any(char.IsLetter);

            return hasDigit && hasLetter;
        }
    }
}