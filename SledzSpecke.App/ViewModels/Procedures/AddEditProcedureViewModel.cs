using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.App.ViewModels.MedicalShifts;

namespace SledzSpecke.App.ViewModels.Procedures
{
    [QueryProperty(nameof(ProcedureId), "procedureId")]
    [QueryProperty(nameof(InternshipId), "internshipId")]
    [QueryProperty(nameof(ModuleId), "moduleId")]
    public class AddEditProcedureViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly ISmkVersionStrategy smkStrategy;
        private readonly IDialogService dialogService;

        private int procedureId;
        private int? internshipId;
        private int? moduleId;
        private DateTime date = DateTime.Today;
        private string code = string.Empty;
        private string operatorCode = "A";
        private string location = string.Empty;
        private string patientInitials = string.Empty;
        private string patientGender = string.Empty;
        private string assistantData = string.Empty;
        private string procedureGroup = string.Empty;
        private string status = string.Empty;
        private string performingPerson = string.Empty;
        private bool canSave;
        private bool isOldSmkVersion;
        private string moduleInfo = string.Empty;
        private bool hasModules;
        private int year = 1;

        private ObservableCollection<InternshipListItem> availableInternships;
        private InternshipListItem selectedInternship;
        private ObservableCollection<string> availableProcedureCodes;
        private ObservableCollection<string> availableGenders;
        private ObservableCollection<string> availableStatuses;
        private ObservableCollection<string> availableOperatorCodes;
        private ObservableCollection<int> availableYears;

        public AddEditProcedureViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            ISmkVersionStrategy smkStrategy,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.smkStrategy = smkStrategy;
            this.dialogService = dialogService;

            // Inicjalizacja tytułu
            this.Title = "Nowa procedura";

            // Inicjalizacja komend
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, () => this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            // Określenie wersji SMK
            this.IsOldSmkVersion = this.smkStrategy.GetType().Name.Contains("Old");

            // Inicjalizacja kolekcji
            this.AvailableInternships = new ObservableCollection<InternshipListItem>();
            this.AvailableProcedureCodes = new ObservableCollection<string>();
            this.AvailableGenders = new ObservableCollection<string> { "M", "K" };
            this.AvailableStatuses = new ObservableCollection<string>();
            this.AvailableOperatorCodes = new ObservableCollection<string> { "A", "B" };
            this.AvailableYears = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6 };

            this.InitializePickerOptions();
        }

        #region Properties

        public int ProcedureId
        {
            get => this.procedureId;
            set
            {
                if (this.SetProperty(ref this.procedureId, value) && value > 0)
                {
                    // Aktualizacja tytułu dla trybu edycji
                    this.Title = "Edytuj procedurę";

                    // Wczytanie danych procedury
                    this.LoadProcedureAsync(value).ConfigureAwait(false);
                }
            }
        }

        public int? InternshipId
        {
            get => this.internshipId;
            set
            {
                if (this.SetProperty(ref this.internshipId, value) && value.HasValue)
                {
                    // Wczytaj dostępne kody procedur dla tego stażu
                    this.LoadProcedureCodesAsync(value.Value).ConfigureAwait(false);
                }
            }
        }

        public int? ModuleId
        {
            get => this.moduleId;
            set
            {
                if (this.SetProperty(ref this.moduleId, value) && value.HasValue)
                {
                    // Wczytaj dane o module
                    this.LoadModuleInfoAsync().ConfigureAwait(false);

                    // Wczytaj staże dla tego modułu
                    this.LoadInternshipsAsync().ConfigureAwait(false);
                }
                else
                {
                    // Wczytaj wszystkie staże
                    this.LoadInternshipsAsync().ConfigureAwait(false);
                }
            }
        }

        public DateTime Date
        {
            get => this.date;
            set
            {
                this.SetProperty(ref this.date, value);
                this.ValidateInput();
            }
        }

        public string Code
        {
            get => this.code;
            set
            {
                this.SetProperty(ref this.code, value);
                this.ValidateInput();
            }
        }

        public string OperatorCode
        {
            get => this.operatorCode;
            set
            {
                this.SetProperty(ref this.operatorCode, value);
                this.ValidateInput();
            }
        }

        public string Location
        {
            get => this.location;
            set
            {
                this.SetProperty(ref this.location, value);
                this.ValidateInput();
            }
        }

        public string PatientInitials
        {
            get => this.patientInitials;
            set
            {
                this.SetProperty(ref this.patientInitials, value);
                this.ValidateInput();
            }
        }

        public string PatientGender
        {
            get => this.patientGender;
            set
            {
                this.SetProperty(ref this.patientGender, value);
                this.ValidateInput();
            }
        }

        public int Year
        {
            get => this.year;
            set
            {
                this.SetProperty(ref this.year, value);
                this.ValidateInput();
            }
        }

        public ObservableCollection<int> AvailableYears
        {
            get => this.availableYears;
            set => this.SetProperty(ref this.availableYears, value);
        }

        public string AssistantData
        {
            get => this.assistantData;
            set
            {
                this.SetProperty(ref this.assistantData, value);
                this.ValidateInput();
            }
        }

        public string ProcedureGroup
        {
            get => this.procedureGroup;
            set
            {
                this.SetProperty(ref this.procedureGroup, value);
                this.ValidateInput();
            }
        }

        public string Status
        {
            get => this.status;
            set
            {
                this.SetProperty(ref this.status, value);
                this.ValidateInput();
            }
        }

        public string PerformingPerson
        {
            get => this.performingPerson;
            set
            {
                this.SetProperty(ref this.performingPerson, value);
                this.ValidateInput();
            }
        }

        public bool CanSave
        {
            get => this.canSave;
            set => this.SetProperty(ref this.canSave, value);
        }

        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set => this.SetProperty(ref this.isOldSmkVersion, value);
        }

        public string ModuleInfo
        {
            get => this.moduleInfo;
            set => this.SetProperty(ref this.moduleInfo, value);
        }

        public bool HasModules
        {
            get => this.hasModules;
            set => this.SetProperty(ref this.hasModules, value);
        }

        public ObservableCollection<InternshipListItem> AvailableInternships
        {
            get => this.availableInternships;
            set => this.SetProperty(ref this.availableInternships, value);
        }

        public InternshipListItem SelectedInternship
        {
            get => this.selectedInternship;
            set
            {
                if (this.SetProperty(ref this.selectedInternship, value) && value != null)
                {
                    this.InternshipId = value.InternshipId;
                }
            }
        }

        public ObservableCollection<string> AvailableProcedureCodes
        {
            get => this.availableProcedureCodes;
            set => this.SetProperty(ref this.availableProcedureCodes, value);
        }

        public ObservableCollection<string> AvailableGenders
        {
            get => this.availableGenders;
            set => this.SetProperty(ref this.availableGenders, value);
        }

        public ObservableCollection<string> AvailableStatuses
        {
            get => this.availableStatuses;
            set => this.SetProperty(ref this.availableStatuses, value);
        }

        public ObservableCollection<string> AvailableOperatorCodes
        {
            get => this.availableOperatorCodes;
            set => this.SetProperty(ref this.availableOperatorCodes, value);
        }

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        #region Private Methods

        private void InitializePickerOptions()
        {
            // Pobierz opcje dla pickerów z wybranej strategii SMK
            var statusOptions = this.smkStrategy.GetPickerOptions("AddEditProcedure", "Status");
            this.AvailableStatuses.Clear();
            foreach (var status in statusOptions.Values)
            {
                this.AvailableStatuses.Add(status);
            }

            // Domyślny status
            if (this.AvailableStatuses.Count > 0)
            {
                this.Status = this.AvailableStatuses[0];
            }

            // Domyślny kod operatora
            var operatorOptions = this.smkStrategy.GetPickerOptions("AddEditProcedure", "OperatorCode");
            this.OperatorCode = operatorOptions.Keys.FirstOrDefault() ?? "A";
        }

        private async Task LoadModuleInfoAsync()
        {
            try
            {
                if (!this.moduleId.HasValue)
                {
                    this.ModuleInfo = string.Empty;
                    return;
                }

                // Sprawdź, czy specjalizacja ma moduły
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                this.HasModules = specialization?.HasModules ?? false;

                // Jeśli specjalizacja ma moduły, pobierz dane modułu
                if (this.HasModules)
                {
                    var module = await this.databaseService.GetModuleAsync(this.moduleId.Value);
                    if (module != null)
                    {
                        this.ModuleInfo = $"Ta procedura będzie dodana do modułu: {module.Name}";
                    }
                }
                else
                {
                    this.ModuleInfo = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania informacji o module: {ex.Message}");
                this.ModuleInfo = string.Empty;
            }
        }

        private async Task LoadInternshipsAsync()
        {
            try
            {
                this.AvailableInternships.Clear();

                // Pobierz staże dla obecnego modułu (jeśli podano) lub wszystkie staże
                var internships = await this.databaseService.GetInternshipsAsync(moduleId: this.moduleId);

                // Dodaj staże do listy
                foreach (var internship in internships)
                {
                    var item = new InternshipListItem
                    {
                        InternshipId = internship.InternshipId,
                        DisplayName = $"{internship.InternshipName} - {internship.InstitutionName}",
                    };
                    this.AvailableInternships.Add(item);
                }

                // Jeśli mamy tylko jeden staż, ustaw go jako wybrany
                if (this.AvailableInternships.Count == 1)
                {
                    this.SelectedInternship = this.AvailableInternships[0];
                }
                // Jeśli mamy przekazany internshipId, ustaw go jako wybrany
                else if (this.internshipId.HasValue)
                {
                    this.SelectedInternship = this.AvailableInternships
                        .FirstOrDefault(i => i.InternshipId == this.internshipId.Value);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania staży: {ex.Message}");
            }
        }

        private async Task LoadProcedureCodesAsync(int internshipId)
        {
            try
            {
                this.AvailableProcedureCodes.Clear();

                // Tu będzie logika do pobierania kodów procedur na podstawie wybranego stażu
                // Na razie dodajemy przykładowe kody
                var exampleCodes = new List<string>
                {
                    "PR001 - Badanie EKG",
                    "PR002 - Nakłucie żyły obwodowej",
                    "PR003 - Nakłucie jamy opłucnej",
                    "PR004 - Resuscytacja krążeniowo-oddechowa",
                    "PR005 - Badanie per rectum",
                    "PR006 - Przetoczenie krwi",
                    "PR007 - Badanie neurologiczne",
                    "PR008 - Intubacja dotchawicza",
                    "PR009 - Kardiowersja elektryczna",
                    "PR010 - Pomiar ciśnienia żylnego"
                };

                foreach (var code in exampleCodes)
                {
                    this.AvailableProcedureCodes.Add(code);
                }

                // TODO: W przyszłości pobierać rzeczywiste kody procedur z bazy danych dla wybranego stażu
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania kodów procedur: {ex.Message}");
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
                // Wczytanie procedury
                var procedure = await this.databaseService.GetProcedureAsync(procedureId);
                if (procedure == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono procedury.",
                        "OK");
                    await this.OnCancelAsync();
                    return;
                }

                // Ustawienie internshipId
                this.internshipId = procedure.InternshipId;

                // Wczytaj staże i ustaw wybrany staż
                await this.LoadInternshipsAsync();
                this.SelectedInternship = this.AvailableInternships
                    .FirstOrDefault(i => i.InternshipId == procedure.InternshipId);

                // Wczytaj kody procedur
                await this.LoadProcedureCodesAsync(procedure.InternshipId);

                // Ustawienie wartości
                this.Date = procedure.Date;
                this.Code = procedure.Code;
                this.OperatorCode = procedure.OperatorCode;
                this.Location = procedure.Location;
                this.PatientInitials = procedure.PatientInitials;
                this.PatientGender = procedure.PatientGender;
                this.AssistantData = procedure.AssistantData;
                this.ProcedureGroup = procedure.ProcedureGroup;
                this.Status = procedure.Status;
                this.PerformingPerson = procedure.PerformingPerson;
                this.Year = procedure.Year;

                // Pobierz staż, aby uzyskać informacje o module
                var internship = await this.databaseService.GetInternshipAsync(procedure.InternshipId);
                if (internship != null && internship.ModuleId.HasValue)
                {
                    this.moduleId = internship.ModuleId.Value;
                    await this.LoadModuleInfoAsync();
                }

                // Walidacja
                this.ValidateInput();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania procedury: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować danych procedury.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void ValidateInput()
        {
            // Sprawdź czy wszystkie wymagane pola są wypełnione
            var requiredFields = this.smkStrategy.GetRequiredFields("AddEditProcedure");

            bool isValid = true;

            // Musi być wybrany staż
            if (this.selectedInternship == null)
            {
                isValid = false;
            }

            // Sprawdź pola wymagane
            if (requiredFields.Contains("Date") && this.Date == default)
            {
                isValid = false;
            }

            if (requiredFields.Contains("Code") && string.IsNullOrWhiteSpace(this.Code))
            {
                isValid = false;
            }

            if (requiredFields.Contains("OperatorCode") && string.IsNullOrWhiteSpace(this.OperatorCode))
            {
                isValid = false;
            }

            if (requiredFields.Contains("Location") && string.IsNullOrWhiteSpace(this.Location))
            {
                isValid = false;
            }

            if (this.IsOldSmkVersion && requiredFields.Contains("Year") && this.Year <= 0)
            {
                isValid = false;
            }

            // Dla starej wersji SMK sprawdź dodatkowe pola
            if (this.IsOldSmkVersion)
            {
                if (requiredFields.Contains("PerformingPerson") && string.IsNullOrWhiteSpace(this.PerformingPerson))
                {
                    isValid = false;
                }
            }

            this.CanSave = isValid;
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy || !this.CanSave || this.SelectedInternship == null)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Utwórz lub pobierz procedurę
                Procedure procedure;
                if (this.ProcedureId > 0)
                {
                    procedure = await this.databaseService.GetProcedureAsync(this.ProcedureId);
                    if (procedure == null)
                    {
                        throw new Exception("Nie znaleziono procedury do edycji.");
                    }

                    // Oznacz jako zmodyfikowane, jeśli było wcześniej zsynchronizowane
                    if (procedure.SyncStatus == SyncStatus.Synced)
                    {
                        procedure.SyncStatus = SyncStatus.Modified;
                    }
                }
                else
                {
                    procedure = new Procedure
                    {
                        SyncStatus = SyncStatus.NotSynced,
                    };
                }

                // Aktualizacja właściwości
                procedure.InternshipId = this.SelectedInternship.InternshipId;
                procedure.Date = this.Date;
                procedure.Code = this.Code;
                procedure.OperatorCode = this.OperatorCode;
                procedure.Location = this.Location;
                procedure.PatientInitials = this.PatientInitials;
                procedure.PatientGender = this.PatientGender;
                procedure.AssistantData = this.AssistantData;
                procedure.ProcedureGroup = this.ProcedureGroup;
                procedure.Status = this.Status;
                procedure.PerformingPerson = this.PerformingPerson;
                procedure.Year = this.Year;

                // Zapisz do bazy danych
                await this.databaseService.SaveProcedureAsync(procedure);

                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.ProcedureId > 0
                        ? "Procedura została pomyślnie zaktualizowana."
                        : "Procedura została pomyślnie dodana.",
                    "OK");

                // Powrót
                await this.OnCancelAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisywania procedury: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać procedury. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        #endregion
    }
}