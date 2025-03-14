using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    [QueryProperty(nameof(ProcedureId), nameof(ProcedureId))]
    [QueryProperty(nameof(RequirementId), nameof(RequirementId))]
    public class AddEditOldSMKProcedureViewModel : BaseViewModel
    {
        private readonly IProcedureService procedureService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private bool isEdit;
        private string procedureId;
        private string requirementId;
        private RealizedProcedureOldSMK procedure;
        private ObservableCollection<KeyValuePair<string, string>> codeOptions;
        private ObservableCollection<KeyValuePair<string, string>> yearOptions;
        private ObservableCollection<KeyValuePair<string, string>> genderOptions;
        private ObservableCollection<Internship> availableInternships;
        private Internship selectedInternship;
        private KeyValuePair<string, string> selectedCode;
        private KeyValuePair<string, string> selectedYear;
        private KeyValuePair<string, string> selectedGender;
        private User currentUser;
        private bool isInitialized;

        // Dodajemy bezpośrednie właściwości na potrzeby wiązania UI
        private string performingPerson;
        private string location;
        private string patientInitials;
        private string assistantData;
        private string procedureGroup;

        public AddEditOldSMKProcedureViewModel(
            IProcedureService procedureService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService)
        {
            this.procedureService = procedureService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.CodeOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.YearOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.GenderOptions = new ObservableCollection<KeyValuePair<string, string>>();
            this.AvailableInternships = new ObservableCollection<Internship>();

            // Inicjalizacja procedury
            this.Procedure = new RealizedProcedureOldSMK
            {
                Date = DateTime.Now,
                Code = "A - operator",
                PatientGender = "K",
                Year = 1,
                SyncStatus = SyncStatus.NotSynced,
                PerformingPerson = string.Empty // Będzie ustawione w InitializeAsync
            };

            // Inicjalizacja bezpośrednich właściwości
            this.PerformingPerson = string.Empty;
            this.Location = string.Empty;
            this.PatientInitials = string.Empty;
            this.AssistantData = string.Empty;
            this.ProcedureGroup = string.Empty;

            // Inicjalizacja komend
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            this.isInitialized = false;
            System.Diagnostics.Debug.WriteLine("AddEditOldSMKProcedureViewModel: Konstruktor zakończony");
        }

        // Nowe bezpośrednie właściwości dla formularza
        public string PerformingPerson
        {
            get => this.performingPerson;
            set
            {
                if (this.SetProperty(ref this.performingPerson, value))
                {
                    this.Procedure.PerformingPerson = value;
                    System.Diagnostics.Debug.WriteLine($"PerformingPerson zmieniony na: {value}");
                }
            }
        }

        public string Location
        {
            get => this.location;
            set
            {
                if (this.SetProperty(ref this.location, value))
                {
                    this.Procedure.Location = value;
                    System.Diagnostics.Debug.WriteLine($"Location zmieniony na: {value}");
                }
            }
        }

        public string PatientInitials
        {
            get => this.patientInitials;
            set
            {
                if (this.SetProperty(ref this.patientInitials, value))
                {
                    this.Procedure.PatientInitials = value;
                    System.Diagnostics.Debug.WriteLine($"PatientInitials zmieniony na: {value}");
                }
            }
        }

        public string AssistantData
        {
            get => this.assistantData;
            set
            {
                if (this.SetProperty(ref this.assistantData, value))
                {
                    this.Procedure.AssistantData = value;
                    System.Diagnostics.Debug.WriteLine($"AssistantData zmieniony na: {value}");
                }
            }
        }

        public string ProcedureGroup
        {
            get => this.procedureGroup;
            set
            {
                if (this.SetProperty(ref this.procedureGroup, value))
                {
                    this.Procedure.ProcedureGroup = value;
                    System.Diagnostics.Debug.WriteLine($"ProcedureGroup zmieniony na: {value}");
                }
            }
        }

        // Właściwości QueryProperty
        public string ProcedureId
        {
            set
            {
                this.procedureId = value;
                System.Diagnostics.Debug.WriteLine($"ProcedureId ustawione na: {value}");
                // Nie wywołujemy tu LoadProcedureAsync - zrobimy to w InitializeAsync
            }
        }

        public string RequirementId
        {
            set
            {
                this.requirementId = value;
                System.Diagnostics.Debug.WriteLine($"RequirementId ustawione na: {value}");
            }
        }

        // Pozostałe właściwości
        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public RealizedProcedureOldSMK Procedure
        {
            get => this.procedure;
            set
            {
                if (this.SetProperty(ref this.procedure, value))
                {
                    // Synchronizuj nasze bezpośrednie właściwości
                    this.SyncPropertiesFromProcedure();
                }
            }
        }

        // Nowa metoda do synchronizacji bezpośrednich właściwości z obiektu Procedure
        private void SyncPropertiesFromProcedure()
        {
            if (this.procedure != null)
            {
                this.PerformingPerson = this.procedure.PerformingPerson ?? string.Empty;
                this.Location = this.procedure.Location ?? string.Empty;
                this.PatientInitials = this.procedure.PatientInitials ?? string.Empty;
                this.AssistantData = this.procedure.AssistantData ?? string.Empty;
                this.ProcedureGroup = this.procedure.ProcedureGroup ?? string.Empty;

                System.Diagnostics.Debug.WriteLine($"SyncPropertiesFromProcedure: " +
                    $"PerformingPerson={this.PerformingPerson}, " +
                    $"Location={this.Location}, " +
                    $"PatientInitials={this.PatientInitials}, " +
                    $"AssistantData={this.AssistantData}, " +
                    $"ProcedureGroup={this.ProcedureGroup}");
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> CodeOptions
        {
            get => this.codeOptions;
            set => this.SetProperty(ref this.codeOptions, value);
        }

        public ObservableCollection<KeyValuePair<string, string>> YearOptions
        {
            get => this.yearOptions;
            set => this.SetProperty(ref this.yearOptions, value);
        }

        public ObservableCollection<KeyValuePair<string, string>> GenderOptions
        {
            get => this.genderOptions;
            set => this.SetProperty(ref this.genderOptions, value);
        }

        public ObservableCollection<Internship> AvailableInternships
        {
            get => this.availableInternships;
            set => this.SetProperty(ref this.availableInternships, value);
        }

        public Internship SelectedInternship
        {
            get => this.selectedInternship;
            set
            {
                if (this.SetProperty(ref this.selectedInternship, value) && value != null)
                {
                    this.Procedure.InternshipId = value.InternshipId;
                    this.Procedure.InternshipName = value.InternshipName;
                    System.Diagnostics.Debug.WriteLine($"SelectedInternship zmieniony na: {value.InternshipName}");
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedCode
        {
            get => this.selectedCode;
            set
            {
                if (this.SetProperty(ref this.selectedCode, value))
                {
                    if (!string.IsNullOrEmpty(value.Key))
                    {
                        this.Procedure.Code = value.Key;
                        System.Diagnostics.Debug.WriteLine($"SelectedCode zmieniony na: {value.Key}");
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value))
                {
                    if (!string.IsNullOrEmpty(value.Key) && int.TryParse(value.Key, out int year))
                    {
                        this.Procedure.Year = year;
                        System.Diagnostics.Debug.WriteLine($"SelectedYear zmieniony na: {value.Key}");
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public KeyValuePair<string, string> SelectedGender
        {
            get => this.selectedGender;
            set
            {
                if (this.SetProperty(ref this.selectedGender, value))
                {
                    if (!string.IsNullOrEmpty(value.Key))
                    {
                        this.Procedure.PatientGender = value.Key;
                        System.Diagnostics.Debug.WriteLine($"SelectedGender zmieniony na: {value.Key}");
                    }
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public User CurrentUser
        {
            get => this.currentUser;
            set => this.SetProperty(ref this.currentUser, value);
        }

        // Komendy
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Metody
        public async Task InitializeAsync()
        {
            if (this.IsBusy || this.isInitialized)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Dodaj dodatkowe informacje diagnostyczne
                System.Diagnostics.Debug.WriteLine("Rozpoczynam inicjalizację AddEditOldSMKProcedureViewModel...");

                // Pobierz aktualnego użytkownika
                this.CurrentUser = await this.authService.GetCurrentUserAsync();
                System.Diagnostics.Debug.WriteLine($"CurrentUser: {this.CurrentUser?.Username ?? "null"}");

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();

                // Sprawdzamy czy to edycja czy dodawanie
                int procId = 0;
                bool isEditMode = !string.IsNullOrEmpty(this.procedureId) && int.TryParse(this.procedureId, out procId) && procId > 0;

                // Jeśli to edycja, ładujemy procedurę przed inicjalizacją dropdown'ów
                if (isEditMode)
                {
                    await this.LoadProcedureAsync(procId);
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj procedurę";

                    if (this.CurrentUser != null && string.IsNullOrEmpty(this.Procedure.PerformingPerson))
                    {
                        this.Procedure.PerformingPerson = this.CurrentUser.Name;
                        this.PerformingPerson = this.CurrentUser.Name;
                        System.Diagnostics.Debug.WriteLine($"Ustawiono PerformingPerson: {this.Procedure.PerformingPerson}");
                    }
                }

                // Załaduj opcje dla dropdownów
                this.LoadDropdownOptions(specialization?.DurationYears ?? 6);

                // Załaduj dostępne staże
                await this.LoadInternshipsAsync();

                // Ustaw wybrane wartości w pickerach na podstawie aktualnych wartości w procedurze
                this.SynchronizePickersWithProcedure();

                // Jeśli to nowa procedura i podano ID wymagania, załaduj dane wymagania
                if (!this.IsEdit && !string.IsNullOrEmpty(this.requirementId) && int.TryParse(this.requirementId, out int reqId))
                {
                    await this.LoadRequirementDataAsync(reqId);
                }

                // Załaduj ostatnią lokalizację dla nowej procedury
                if (!this.IsEdit && string.IsNullOrEmpty(this.Procedure.Location))
                {
                    await this.LoadLastLocationAsync();
                }

                // Upewnij się, że bezpośrednie właściwości są zsynchronizowane
                this.SyncPropertiesFromProcedure();

                // Na koniec powiadom o możliwości zapisania
                ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                System.Diagnostics.Debug.WriteLine("Zakończono inicjalizację AddEditOldSMKProcedureViewModel");

                this.isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Wystąpił problem podczas inicjalizacji: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        // Metoda synchronizująca pickery z modelem procedury
        private void SynchronizePickersWithProcedure()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SynchronizePickersWithProcedure: Synchronizacja pickerów z procedurą");

                // Synchronizacja kodu
                var codeItem = this.CodeOptions.FirstOrDefault(c => c.Key == this.Procedure.Code);
                if (codeItem.Key != null)
                {
                    this.SelectedCode = codeItem;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono SelectedCode: {codeItem.Key}");
                }
                else if (this.CodeOptions.Count > 0)
                {
                    this.SelectedCode = this.CodeOptions.First();
                    System.Diagnostics.Debug.WriteLine($"Nie znaleziono kodu procedury, ustawiono domyślny: {this.CodeOptions.First().Key}");
                }

                // Synchronizacja roku
                var yearItem = this.YearOptions.FirstOrDefault(y => y.Key == this.Procedure.Year.ToString());
                if (yearItem.Key != null)
                {
                    this.SelectedYear = yearItem;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono SelectedYear: {yearItem.Key}");
                }
                else if (this.YearOptions.Count > 0)
                {
                    this.SelectedYear = this.YearOptions.First();
                    System.Diagnostics.Debug.WriteLine($"Nie znaleziono roku procedury, ustawiono domyślny: {this.YearOptions.First().Key}");
                }

                // Synchronizacja płci
                var genderItem = this.GenderOptions.FirstOrDefault(g => g.Key == this.Procedure.PatientGender);
                if (genderItem.Key != null)
                {
                    this.SelectedGender = genderItem;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono SelectedGender: {genderItem.Key}");
                }
                else if (this.GenderOptions.Count > 0)
                {
                    this.SelectedGender = this.GenderOptions.First();
                    System.Diagnostics.Debug.WriteLine($"Nie znaleziono płci pacjenta, ustawiono domyślną: {this.GenderOptions.First().Key}");
                }

                // Synchronizacja stażu
                var internship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == this.Procedure.InternshipId);
                if (internship != null)
                {
                    this.SelectedInternship = internship;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono SelectedInternship: {internship.InternshipName}");
                }
                else if (this.AvailableInternships.Count > 0)
                {
                    this.SelectedInternship = this.AvailableInternships.First();
                    System.Diagnostics.Debug.WriteLine($"Nie znaleziono stażu procedury, ustawiono domyślny: {this.AvailableInternships.First().InternshipName}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas synchronizacji pickerów: {ex.Message}");
            }
        }

        // Metoda do ładowania opcji dropdownów
        private void LoadDropdownOptions(int yearsFromSpecialization)
        {
            try
            {
                // Opcje kodu
                this.CodeOptions.Clear();
                this.CodeOptions.Add(new KeyValuePair<string, string>("A - operator", "A - operator"));
                this.CodeOptions.Add(new KeyValuePair<string, string>("B - asysta", "B - asysta"));
                System.Diagnostics.Debug.WriteLine($"Załadowano opcje kodu, liczba opcji: {this.CodeOptions.Count}");

                // Opcje płci
                this.GenderOptions.Clear();
                this.GenderOptions.Add(new KeyValuePair<string, string>("K", "K"));
                this.GenderOptions.Add(new KeyValuePair<string, string>("M", "M"));
                System.Diagnostics.Debug.WriteLine($"Załadowano opcje płci, liczba opcji: {this.GenderOptions.Count}");

                // Opcje roku
                this.YearOptions.Clear();
                for (int i = 1; i <= yearsFromSpecialization; i++)
                {
                    this.YearOptions.Add(new KeyValuePair<string, string>(i.ToString(), $"Rok {i}"));
                }
                System.Diagnostics.Debug.WriteLine($"Załadowano opcje roku, liczba opcji: {this.YearOptions.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania opcji dropdownów: {ex.Message}");
            }
        }

        private async Task LoadInternshipsAsync()
        {
            try
            {
                // Pobierz aktualny moduł
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                System.Diagnostics.Debug.WriteLine($"Pobrano moduł: {currentModule?.Name ?? "null"}");

                // Pobierz staże dla bieżącego modułu
                var internships = await this.specializationService.GetInternshipsAsync(moduleId: currentModule?.ModuleId);
                System.Diagnostics.Debug.WriteLine($"Pobrano {internships.Count} staży");

                this.AvailableInternships.Clear();
                foreach (var internship in internships)
                {
                    this.AvailableInternships.Add(internship);
                    System.Diagnostics.Debug.WriteLine($"Dodano staż: ID={internship.InternshipId}, Name={internship.InternshipName}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania staży: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        // Metoda do ładowania danych wymagania
        private async Task LoadRequirementDataAsync(int reqId)
        {
            try
            {
                var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync();
                var requirement = requirements.FirstOrDefault(r => r.Id == reqId);
                System.Diagnostics.Debug.WriteLine($"Znaleziono wymaganie: {requirement?.Name ?? "null"}");

                if (requirement != null)
                {
                    // Znajdź staż powiązany z wymaganiem
                    if (requirement.InternshipId.HasValue)
                    {
                        var internship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == requirement.InternshipId.Value);
                        if (internship != null)
                        {
                            this.SelectedInternship = internship;
                            System.Diagnostics.Debug.WriteLine($"Ustawiono SelectedInternship: {internship.InternshipName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania danych wymagania: {ex.Message}");
            }
        }

        private async Task LoadProcedureAsync(int procedureId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobierz procedurę
                var loadedProcedure = await this.procedureService.GetOldSMKProcedureAsync(procedureId);
                if (loadedProcedure != null)
                {
                    this.IsEdit = true;
                    this.Title = "Edytuj procedurę";

                    // Zapisz referencję do obiektu procedury
                    this.Procedure = loadedProcedure;

                    // Jawne przypisanie każdej właściwości, aby upewnić się, że dane są prawidłowo przesyłane
                    this.PerformingPerson = loadedProcedure.PerformingPerson ?? string.Empty;
                    this.Location = loadedProcedure.Location ?? string.Empty;
                    this.PatientInitials = loadedProcedure.PatientInitials ?? string.Empty;
                    this.AssistantData = loadedProcedure.AssistantData ?? string.Empty;
                    this.ProcedureGroup = loadedProcedure.ProcedureGroup ?? string.Empty;

                    System.Diagnostics.Debug.WriteLine($"Załadowano procedurę: {loadedProcedure.ProcedureId}, " +
                        $"Code={loadedProcedure.Code}, Year={loadedProcedure.Year}, " +
                        $"PerformingPerson={this.PerformingPerson}, Location={this.Location}, " +
                        $"PatientInitials={this.PatientInitials}, PatientGender={loadedProcedure.PatientGender}, " +
                        $"AssistantData={this.AssistantData}, ProcedureGroup={this.ProcedureGroup}, " +
                        $"InternshipId={loadedProcedure.InternshipId}");
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj procedurę";
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono procedury o podanym identyfikatorze.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania procedury: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania procedury.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadLastLocationAsync()
        {
            try
            {
                // Pobierz użytkownika
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return;
                }

                // Pobierz ostatnią procedurę
                var lastProcedures = await this.procedureService.GetOldSMKProceduresAsync();

                if (lastProcedures.Count > 0)
                {
                    // Ustaw miejsce wykonania z ostatniej procedury
                    this.Procedure.Location = lastProcedures[0].Location;
                    this.Location = lastProcedures[0].Location;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono Location z ostatniej procedury: {this.Procedure.Location}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania ostatniej lokalizacji: {ex.Message}");
            }
        }

        private bool ValidateInputs()
        {
            System.Diagnostics.Debug.WriteLine("ValidateInputs: Rozpoczynam walidację...");

            // Sprawdzamy, czy Procedure jest null
            if (this.Procedure == null)
            {
                System.Diagnostics.Debug.WriteLine("ValidateInputs: Procedure jest null");
                return false;
            }

            // Sprawdź wszystkie pola
            bool isProcedureValid = this.Procedure != null;
            bool isCodeValid = !string.IsNullOrWhiteSpace(this.Procedure.Code);
            bool isLocationValid = !string.IsNullOrWhiteSpace(this.Location); // Zmiana na bezpośrednią właściwość
            bool isPersonValid = !string.IsNullOrWhiteSpace(this.PerformingPerson); // Zmiana na bezpośrednią właściwość
            bool isInitialsValid = !string.IsNullOrWhiteSpace(this.PatientInitials); // Zmiana na bezpośrednią właściwość
            bool isGenderValid = !string.IsNullOrWhiteSpace(this.Procedure.PatientGender);
            bool isInternshipValid = this.SelectedInternship != null;

            System.Diagnostics.Debug.WriteLine($"ValidateInputs: Procedure={isProcedureValid}, Code={isCodeValid}, " +
                $"Location={isLocationValid}, Person={isPersonValid}, Initials={isInitialsValid}, " +
                $"Gender={isGenderValid}, Internship={isInternshipValid}");

            bool result = isProcedureValid && isCodeValid && isLocationValid &&
                   isPersonValid && isInitialsValid && isGenderValid && isInternshipValid;

            System.Diagnostics.Debug.WriteLine($"ValidateInputs: Wynik={result}");
            return result;
        }

        public async Task OnSaveAsync()
        {
            System.Diagnostics.Debug.WriteLine("OnSaveAsync: Metoda wywołana");

            if (this.IsBusy)
            {
                System.Diagnostics.Debug.WriteLine("OnSaveAsync: Metoda jest już zajęta (IsBusy=true)");
                return;
            }

            // Wykonaj walidację wewnątrz metody zamiast używać CanSave
            if (!this.ValidateInputs())
            {
                System.Diagnostics.Debug.WriteLine("OnSaveAsync: Walidacja nie powiodła się");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd walidacji",
                    "Proszę wypełnić wszystkie wymagane pola przed zapisaniem procedury.",
                    "OK");
                return;
            }

            this.IsBusy = true;
            System.Diagnostics.Debug.WriteLine("OnSaveAsync: Rozpoczynam zapisywanie procedury...");

            try
            {
                // Upewnij się, że wszystkie wartości z bezpośrednich właściwości są skopiowane do procedury
                this.Procedure.PerformingPerson = this.PerformingPerson;
                this.Procedure.Location = this.Location;
                this.Procedure.PatientInitials = this.PatientInitials;
                this.Procedure.AssistantData = this.AssistantData;
                this.Procedure.ProcedureGroup = this.ProcedureGroup;

                // Upewnij się, że specjalizacja jest ustawiona
                if (this.Procedure.SpecializationId <= 0 && this.CurrentUser != null)
                {
                    this.Procedure.SpecializationId = this.CurrentUser.SpecializationId;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono SpecializationId na {this.Procedure.SpecializationId}");
                }

                // Upewnij się, że InternshipId jest poprawnie ustawiony
                if (this.SelectedInternship != null)
                {
                    this.Procedure.InternshipId = this.SelectedInternship.InternshipId;
                    this.Procedure.InternshipName = this.SelectedInternship.InternshipName;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono InternshipId na {this.Procedure.InternshipId}, InternshipName na {this.Procedure.InternshipName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("BŁĄD: SelectedInternship jest null!");
                }

                // Ustawienie ProcedureRequirementId, jeśli przekazano
                if (!string.IsNullOrEmpty(this.requirementId) && int.TryParse(this.requirementId, out int reqId))
                {
                    this.Procedure.ProcedureRequirementId = reqId;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono ProcedureRequirementId na {reqId}");
                }

                System.Diagnostics.Debug.WriteLine("Zapisuję procedurę...");
                System.Diagnostics.Debug.WriteLine($"Procedura: ID={this.Procedure.ProcedureId}, Code={this.Procedure.Code}, " +
                    $"Location={this.Procedure.Location}, Date={this.Procedure.Date}, " +
                    $"PerformingPerson={this.Procedure.PerformingPerson}, PatientInitials={this.Procedure.PatientInitials}");

                // Zapisz procedurę
                bool success = await this.procedureService.SaveOldSMKProcedureAsync(this.Procedure);
                System.Diagnostics.Debug.WriteLine($"Wynik zapisywania: {success}");

                if (success)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        this.IsEdit ? "Procedura została zaktualizowana." : "Procedura została dodana.",
                        "OK");

                    // Wróć do poprzedniej strony
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zapisać procedury. Sprawdź poprawność danych.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania procedury: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Wystąpił problem podczas zapisywania procedury: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
                System.Diagnostics.Debug.WriteLine("OnSaveAsync: Zakończono (IsBusy=false)");
            }
        }

        private async Task OnCancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}