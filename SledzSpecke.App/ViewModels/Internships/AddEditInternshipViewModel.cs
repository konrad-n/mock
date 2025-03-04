using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    [QueryProperty(nameof(InternshipId), "internshipId")]
    [QueryProperty(nameof(ModuleId), "moduleId")]
    public class AddEditInternshipViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private int internshipId;
        private int? moduleId;
        private string internshipName = string.Empty;
        private string institutionName = string.Empty;
        private string departmentName = string.Empty;
        private DateTime startDate = DateTime.Today;
        private DateTime endDate = DateTime.Today.AddMonths(3);
        private int year = 1;
        private bool isCompleted;
        private bool isApproved;
        private bool canSave;
        private string moduleInfo = string.Empty;
        private bool hasModules;

        private ObservableCollection<string> availableInternshipTypes;
        private string selectedInternshipType;
        private ObservableCollection<int> availableYears;

        public AddEditInternshipViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja tytułu
            this.Title = "Nowy staż";

            // Inicjalizacja komend
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, () => this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            // Inicjalizacja kolekcji
            this.AvailableInternshipTypes = new ObservableCollection<string>
            {
                "Staż podstawowy w zakresie chorób wewnętrznych",
                "Staż kierunkowy w zakresie intensywnej opieki medycznej",
                "Staż kierunkowy w szpitalnym oddziale ratunkowym",
                "Staż kierunkowy w zakresie kardiologii",
                "Staż kierunkowy w zakresie elektrofizjologii",
                "Staż kierunkowy w zakresie kardiologii interwencyjnej",
                "Staż kierunkowy w zakresie kardiochirurgii",
                "Staż kierunkowy w zakresie neurologii",
                "Staż kierunkowy w zakresie psychiatrii",
                "Inny staż kierunkowy",
            };

            this.AvailableYears = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6 };
        }

        // Właściwości
        public int InternshipId
        {
            get => this.internshipId;
            set
            {
                if (this.SetProperty(ref this.internshipId, value) && value > 0)
                {
                    // Aktualizacja tytułu dla trybu edycji
                    this.Title = "Edytuj staż";

                    // Wczytanie danych stażu
                    this.LoadInternshipAsync(value).ConfigureAwait(false);
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
                }
            }
        }

        public string InternshipName
        {
            get => this.internshipName;
            set
            {
                this.SetProperty(ref this.internshipName, value);
                this.ValidateInput();
            }
        }

        public string InstitutionName
        {
            get => this.institutionName;
            set
            {
                this.SetProperty(ref this.institutionName, value);
                this.ValidateInput();
            }
        }

        public string DepartmentName
        {
            get => this.departmentName;
            set
            {
                this.SetProperty(ref this.departmentName, value);
                this.ValidateInput();
            }
        }

        public DateTime StartDate
        {
            get => this.startDate;
            set
            {
                if (this.SetProperty(ref this.startDate, value))
                {
                    this.UpdateDuration();
                }
            }
        }

        public DateTime EndDate
        {
            get => this.endDate;
            set
            {
                if (this.SetProperty(ref this.endDate, value))
                {
                    this.UpdateDuration();
                }
            }
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public bool IsCompleted
        {
            get => this.isCompleted;
            set => this.SetProperty(ref this.isCompleted, value);
        }

        public bool IsApproved
        {
            get => this.isApproved;
            set => this.SetProperty(ref this.isApproved, value);
        }

        public bool CanSave
        {
            get => this.canSave;
            set => this.SetProperty(ref this.canSave, value);
        }

        public ObservableCollection<string> AvailableInternshipTypes
        {
            get => this.availableInternshipTypes;
            set => this.SetProperty(ref this.availableInternshipTypes, value);
        }

        public string SelectedInternshipType
        {
            get => this.selectedInternshipType;
            set
            {
                if (this.SetProperty(ref this.selectedInternshipType, value))
                {
                    this.InternshipName = value;
                }
            }
        }

        public ObservableCollection<int> AvailableYears
        {
            get => this.availableYears;
            set => this.SetProperty(ref this.availableYears, value);
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

        public int DurationDays { get; private set; }

        // Komendy
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Metody
        private async Task LoadInternshipAsync(int internshipId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Wczytanie stażu
                var internship = await this.databaseService.GetInternshipAsync(internshipId);
                if (internship == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono stażu.",
                        "OK");
                    await this.OnCancelAsync();
                    return;
                }

                // Ustawienie moduleId na podstawie stażu
                this.moduleId = internship.ModuleId;

                // Ustawienie właściwości
                this.InternshipName = internship.InternshipName;
                this.InstitutionName = internship.InstitutionName;
                this.DepartmentName = internship.DepartmentName;
                this.StartDate = internship.StartDate;
                this.EndDate = internship.EndDate;
                this.Year = internship.Year;
                this.IsCompleted = internship.IsCompleted;
                this.IsApproved = internship.IsApproved;

                // Spróbuj znaleźć odpowiadający typ stażu
                this.SelectedInternshipType = this.AvailableInternshipTypes
                    .FirstOrDefault(t => t == internship.InternshipName) ?? string.Empty;

                // Wczytaj dane o module, jeśli istnieje
                if (internship.ModuleId.HasValue)
                {
                    await this.LoadModuleInfoAsync();
                }

                // Walidacja
                this.ValidateInput();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania stażu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować danych stażu.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
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
                        this.ModuleInfo = $"Ten staż będzie dodany do modułu: {module.Name}";
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

        private void UpdateDuration()
        {
            // Oblicz liczbę dni między StartDate i EndDate
            if (this.EndDate >= this.StartDate)
            {
                // Liczba dni włącznie z dniem rozpoczęcia i zakończenia
                int days = (this.EndDate - this.StartDate).Days + 1;
                this.DurationDays = days;
            }
            else
            {
                this.DurationDays = 0;
            }

            // Walidacja po zmianie dat
            this.ValidateInput();
        }

        private void ValidateInput()
        {
            // Sprawdź czy wszystkie wymagane pola są wypełnione
            this.CanSave = !string.IsNullOrWhiteSpace(this.InternshipName)
                        && !string.IsNullOrWhiteSpace(this.InstitutionName)
                        && this.EndDate >= this.StartDate;
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy || !this.CanSave)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobierz aktualną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new Exception("Nie znaleziono aktywnej specjalizacji.");
                }

                // Utwórz lub pobierz staż
                Internship internship;
                if (this.InternshipId > 0)
                {
                    internship = await this.databaseService.GetInternshipAsync(this.InternshipId);
                    if (internship == null)
                    {
                        throw new Exception("Nie znaleziono stażu do edycji.");
                    }

                    // Oznacz jako zmodyfikowane, jeśli było wcześniej zsynchronizowane
                    if (internship.SyncStatus == SyncStatus.Synced)
                    {
                        internship.SyncStatus = SyncStatus.Modified;
                    }
                }
                else
                {
                    internship = new Internship
                    {
                        SpecializationId = specialization.SpecializationId,
                        SyncStatus = SyncStatus.NotSynced,
                    };
                }

                // Aktualizacja właściwości
                internship.InternshipName = this.InternshipName;
                internship.InstitutionName = this.InstitutionName;
                internship.DepartmentName = this.DepartmentName;
                internship.StartDate = this.StartDate;
                internship.EndDate = this.EndDate;
                internship.Year = this.Year;
                internship.IsCompleted = this.IsCompleted;
                internship.IsApproved = this.IsApproved;
                internship.DaysCount = this.DurationDays;

                // Ustaw moduł, jeśli specjalizacja ma moduły
                if (specialization.HasModules)
                {
                    // Jeśli przekazano moduleId, użyj go
                    if (this.moduleId.HasValue)
                    {
                        internship.ModuleId = this.moduleId.Value;
                    }
                    // W przeciwnym razie użyj bieżącego modułu specjalizacji
                    else if (specialization.CurrentModuleId.HasValue)
                    {
                        internship.ModuleId = specialization.CurrentModuleId.Value;
                    }
                }
                else
                {
                    internship.ModuleId = null;
                }

                // Zapisz do bazy danych
                await this.databaseService.SaveInternshipAsync(internship);

                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.InternshipId > 0
                        ? "Staż został pomyślnie zaktualizowany."
                        : "Staż został pomyślnie dodany.",
                    "OK");

                // Powrót
                await this.OnCancelAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisywania stażu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać stażu. Spróbuj ponownie.",
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
    }
}