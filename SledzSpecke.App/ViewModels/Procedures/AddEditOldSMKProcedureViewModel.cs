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
        private string selectedCode;
        private string selectedYear;
        private string selectedGender;
        private User currentUser;

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
                PerformingPerson = this.authService.GetCurrentUserAsync().ConfigureAwait(false).GetAwaiter().GetResult()?.Name
            };

            // Inicjalizacja komend - KLUCZOWA ZMIANA: usuń walidację przy inicjalizacji
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            System.Diagnostics.Debug.WriteLine("AddEditOldSMKProcedureViewModel: Konstruktor zakończony, SaveCommand utworzone");
        }

        // Właściwości QueryProperty
        public string ProcedureId
        {
            set
            {
                this.procedureId = value;
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.LoadProcedureAsync(id).ConfigureAwait(false);
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj procedurę";
                }
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
            set => this.SetProperty(ref this.procedure, value);
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
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string SelectedCode
        {
            get => this.selectedCode;
            set
            {
                if (this.SetProperty(ref this.selectedCode, value) && !string.IsNullOrEmpty(value))
                {
                    this.Procedure.Code = value;
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value) && !string.IsNullOrEmpty(value))
                {
                    this.Procedure.Year = int.Parse(value);
                    ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string SelectedGender
        {
            get => this.selectedGender;
            set
            {
                if (this.SetProperty(ref this.selectedGender, value) && !string.IsNullOrEmpty(value))
                {
                    this.Procedure.PatientGender = value;
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
            if (this.IsBusy)
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

                if (this.CurrentUser != null && string.IsNullOrEmpty(this.Procedure.PerformingPerson))
                {
                    this.Procedure.PerformingPerson = this.CurrentUser.Name;
                    System.Diagnostics.Debug.WriteLine($"Ustawiono PerformingPerson: {this.Procedure.PerformingPerson}");
                }

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();

                // Załaduj opcje dla dropdownów
                this.LoadDropdownOptions(specialization.DurationYears);

                // Załaduj dostępne staże
                await this.LoadInternshipsAsync();

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

                // Na koniec powiadom o możliwości zapisania
                ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
                System.Diagnostics.Debug.WriteLine("Zakończono inicjalizację AddEditOldSMKProcedureViewModel");
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

        // Nowa metoda do ładowania opcji dropdownów
        private void LoadDropdownOptions(int yearsFromSpecialization)
        {
            try
            {
                // Opcje kodu
                this.CodeOptions.Clear();
                this.CodeOptions.Add(new KeyValuePair<string, string>("A - operator", "A - operator"));
                this.CodeOptions.Add(new KeyValuePair<string, string>("B - asysta", "B - asysta"));
                this.SelectedCode = this.Procedure.Code;
                System.Diagnostics.Debug.WriteLine($"Załadowano opcje kodu, SelectedCode: {this.SelectedCode}");

                // Opcje płci
                this.GenderOptions.Clear();
                this.GenderOptions.Add(new KeyValuePair<string, string>("K", "K"));
                this.GenderOptions.Add(new KeyValuePair<string, string>("M", "M"));
                this.SelectedGender = this.Procedure.PatientGender;
                System.Diagnostics.Debug.WriteLine($"Załadowano opcje płci, SelectedGender: {this.SelectedGender}");

                // Opcje roku

                this.YearOptions.Clear();

                for (int i = 1; i <= yearsFromSpecialization; i++)
                {
                    this.YearOptions.Add(new KeyValuePair<string, string>(i.ToString(), $"Rok {i}"));
                }
                this.SelectedYear = this.Procedure.Year.ToString();
                System.Diagnostics.Debug.WriteLine($"Załadowano opcje roku, SelectedYear: {this.SelectedYear}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania opcji dropdownów: {ex.Message}");
            }
        }

        // Nowa metoda do ładowania danych wymagania
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
                    this.Procedure = loadedProcedure;
                    System.Diagnostics.Debug.WriteLine($"Załadowano procedurę: {loadedProcedure.ProcedureId}");
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

                // Jeśli edytujemy procedurę, wybierz odpowiedni staż
                if (this.IsEdit && this.Procedure.InternshipId > 0)
                {
                    var selectedInternship = this.AvailableInternships.FirstOrDefault(i => i.InternshipId == this.Procedure.InternshipId);
                    if (selectedInternship != null)
                    {
                        this.SelectedInternship = selectedInternship;
                        System.Diagnostics.Debug.WriteLine($"Wybrano staż dla edycji: {selectedInternship.InternshipName}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Nie znaleziono stażu o ID={this.Procedure.InternshipId}");
                    }
                }
                // Jeśli dodajemy procedurę, wybierz pierwszy staż
                else if (this.AvailableInternships.Count > 0)
                {
                    this.SelectedInternship = this.AvailableInternships[0];
                    System.Diagnostics.Debug.WriteLine($"Wybrano pierwszy staż: {this.SelectedInternship.InternshipName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Brak dostępnych staży!");
                }

                // Powiadom o możliwości zapisania
                ((AsyncRelayCommand)this.SaveCommand).NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania staży: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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
                var query = "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ? ORDER BY ProcedureId DESC LIMIT 1";
                var lastProcedures = await this.procedureService.GetOldSMKProceduresAsync();

                if (lastProcedures.Count > 0)
                {
                    // Ustaw miejsce wykonania z ostatniej procedury
                    this.Procedure.Location = lastProcedures[0].Location;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania ostatniej lokalizacji: {ex.Message}");
            }
        }

        private bool CanSave()
        {
            // Dodaj debugowanie, które pokazuje które pole jest niepoprawne
            bool isProcedureValid = this.Procedure != null;
            bool isCodeValid = !string.IsNullOrWhiteSpace(this.Procedure?.Code);
            bool isLocationValid = !string.IsNullOrWhiteSpace(this.Procedure?.Location);
            bool isPersonValid = !string.IsNullOrWhiteSpace(this.Procedure?.PerformingPerson);
            bool isInitialsValid = !string.IsNullOrWhiteSpace(this.Procedure?.PatientInitials);
            bool isGenderValid = !string.IsNullOrWhiteSpace(this.Procedure?.PatientGender);
            bool isInternshipValid = this.SelectedInternship != null;

            System.Diagnostics.Debug.WriteLine($"CanSave check: Procedure={isProcedureValid}, Code={isCodeValid}, Location={isLocationValid}, " +
                $"Person={isPersonValid}, Initials={isInitialsValid}, Gender={isGenderValid}, Internship={isInternshipValid}");

            return isProcedureValid && isCodeValid && isLocationValid &&
                   isPersonValid && isInitialsValid && isGenderValid && isInternshipValid;
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
            bool isLocationValid = !string.IsNullOrWhiteSpace(this.Procedure.Location);
            bool isPersonValid = !string.IsNullOrWhiteSpace(this.Procedure.PerformingPerson);
            bool isInitialsValid = !string.IsNullOrWhiteSpace(this.Procedure.PatientInitials);
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

        private async Task OnCancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}