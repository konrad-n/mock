using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Dashboard
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        // Current module selection
        private int currentModuleId;
        private Models.Specialization currentSpecialization;
        private Module currentModule;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private bool hasModules;
        private ObservableCollection<ModuleInfo> availableModules;

        // Progress statistics
        private double overallProgress;
        private double internshipProgress;
        private double courseProgress;
        private double procedureProgress;
        private double shiftProgress;

        // Counts
        private string internshipCount;
        private string procedureCount;
        private string courseCount;
        private string shiftStats;
        private int selfEducationCount;
        private int publicationCount;

        // Descriptive information
        private string moduleTitle;
        private string specializationInfo;
        private string dateRangeInfo;
        private string progressText;

        public DashboardViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Initialize collections
            this.AvailableModules = new ObservableCollection<ModuleInfo>();

            // Initialize commands
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);
            this.NavigateToInternshipsCommand = new AsyncRelayCommand(this.NavigateToInternshipsAsync);
            this.NavigateToProceduresCommand = new AsyncRelayCommand(this.NavigateToProceduresAsync);
            this.NavigateToShiftsCommand = new AsyncRelayCommand(this.NavigateToShiftsAsync);
            this.NavigateToCoursesCommand = new AsyncRelayCommand(this.NavigateToCoursesAsync);
            this.NavigateToSelfEducationCommand = new AsyncRelayCommand(this.NavigateToSelfEducationAsync);
            this.NavigateToPublicationsCommand = new AsyncRelayCommand(this.NavigateToPublicationsAsync);
            this.NavigateToAbsencesCommand = new AsyncRelayCommand(this.NavigateToAbsencesAsync);
            this.NavigateToStatisticsCommand = new AsyncRelayCommand(this.NavigateToStatisticsAsync);
            this.NavigateToExportCommand = new AsyncRelayCommand(this.NavigateToExportAsync);
            this.NavigateToRecognitionsCommand = new AsyncRelayCommand(this.NavigateToRecognitionsAsync);

            // Load data on initialization
            this.LoadDataAsync().ConfigureAwait(false);
        }

        // Properties
        public int CurrentModuleId
        {
            get => this.currentModuleId;
            set
            {
                if (this.SetProperty(ref this.currentModuleId, value))
                {
                    // Reload all data with new module filter
                    this.LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        public bool HasModules
        {
            get => this.hasModules;
            set => this.SetProperty(ref this.hasModules, value);
        }

        public Models.Specialization CurrentSpecialization
        {
            get => this.currentSpecialization;
            set => this.SetProperty(ref this.currentSpecialization, value);
        }

        public Module CurrentModule
        {
            get => this.currentModule;
            set => this.SetProperty(ref this.currentModule, value);
        }

        public ObservableCollection<ModuleInfo> AvailableModules
        {
            get => this.availableModules;
            set => this.SetProperty(ref this.availableModules, value);
        }

        public bool BasicModuleSelected
        {
            get => this.basicModuleSelected;
            set => this.SetProperty(ref this.basicModuleSelected, value);
        }

        public bool SpecialisticModuleSelected
        {
            get => this.specialisticModuleSelected;
            set => this.SetProperty(ref this.specialisticModuleSelected, value);
        }

        public double OverallProgress
        {
            get => this.overallProgress;
            set => this.SetProperty(ref this.overallProgress, value);
        }

        public double InternshipProgress
        {
            get => this.internshipProgress;
            set => this.SetProperty(ref this.internshipProgress, value);
        }

        public double CourseProgress
        {
            get => this.courseProgress;
            set => this.SetProperty(ref this.courseProgress, value);
        }

        public double ProcedureProgress
        {
            get => this.procedureProgress;
            set => this.SetProperty(ref this.procedureProgress, value);
        }

        public double ShiftProgress
        {
            get => this.shiftProgress;
            set => this.SetProperty(ref this.shiftProgress, value);
        }

        public string InternshipCount
        {
            get => this.internshipCount;
            set => this.SetProperty(ref this.internshipCount, value);
        }

        public string ProcedureCount
        {
            get => this.procedureCount;
            set => this.SetProperty(ref this.procedureCount, value);
        }

        public string CourseCount
        {
            get => this.courseCount;
            set => this.SetProperty(ref this.courseCount, value);
        }

        public string ShiftStats
        {
            get => this.shiftStats;
            set => this.SetProperty(ref this.shiftStats, value);
        }

        public int SelfEducationCount
        {
            get => this.selfEducationCount;
            set => this.SetProperty(ref this.selfEducationCount, value);
        }

        public int PublicationCount
        {
            get => this.publicationCount;
            set => this.SetProperty(ref this.publicationCount, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public string SpecializationInfo
        {
            get => this.specializationInfo;
            set => this.SetProperty(ref this.specializationInfo, value);
        }

        public string DateRangeInfo
        {
            get => this.dateRangeInfo;
            set => this.SetProperty(ref this.dateRangeInfo, value);
        }

        public string ProgressText
        {
            get => this.progressText;
            set => this.SetProperty(ref this.progressText, value);
        }

        // Commands
        public ICommand RefreshCommand { get; }

        public ICommand SelectModuleCommand { get; }

        public ICommand NavigateToInternshipsCommand { get; }

        public ICommand NavigateToProceduresCommand { get; }

        public ICommand NavigateToShiftsCommand { get; }

        public ICommand NavigateToCoursesCommand { get; }

        public ICommand NavigateToSelfEducationCommand { get; }

        public ICommand NavigateToPublicationsCommand { get; }

        public ICommand NavigateToAbsencesCommand { get; }

        public ICommand NavigateToStatisticsCommand { get; }

        public ICommand NavigateToExportCommand { get; }

        public ICommand NavigateToRecognitionsCommand { get; }

        // Methods
        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            System.Diagnostics.Debug.WriteLine("DashboardViewModel: Rozpoczęto ładowanie danych");

            try
            {
                // Load current specialization
                this.CurrentSpecialization = await this.specializationService.GetCurrentSpecializationAsync();
                System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Pobrano specjalizację: {this.CurrentSpecialization?.Name ?? "null"}");

                if (this.CurrentSpecialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono aktywnej specjalizacji. Proszę skontaktować się z administratorem.",
                        "OK");
                    System.Diagnostics.Debug.WriteLine("DashboardViewModel: Nie znaleziono specjalizacji");
                    return;
                }

                // Ustawienie flagi HasModules
                this.HasModules = this.CurrentSpecialization.HasModules;
                System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Specjalizacja ma moduły: {this.HasModules}");

                // Jeśli specjalizacja ma moduły, inicjalizuj je jeśli nie istnieją
                if (this.CurrentSpecialization.HasModules)
                {
                    // Upewnij się, że moduły są zainicjalizowane
                    await this.specializationService.InitializeSpecializationModulesAsync(this.CurrentSpecialization.SpecializationId);

                    // Załaduj wszystkie moduły
                    var modules = await this.databaseService.GetModulesAsync(this.CurrentSpecialization.SpecializationId);
                    System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Pobrano {modules.Count} modułów");

                    // Odśwież listę dostępnych modułów
                    this.AvailableModules.Clear();
                    foreach (var module in modules)
                    {
                        this.AvailableModules.Add(new ModuleInfo
                        {
                            Id = module.ModuleId,
                            Name = module.Name,
                        });
                        System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Dodano moduł {module.Name}");
                    }

                    // Jeśli nie ustawiono bieżącego modułu, użyj zapisanego w specjalizacji lub pierwszego z listy
                    if (this.CurrentModuleId == 0)
                    {
                        // Najpierw sprawdź, czy specjalizacja ma zdefiniowany bieżący moduł
                        if (this.CurrentSpecialization.CurrentModuleId.HasValue && this.CurrentSpecialization.CurrentModuleId.Value > 0)
                        {
                            this.CurrentModuleId = this.CurrentSpecialization.CurrentModuleId.Value;
                            System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Ustawiono moduł z ID specjalizacji: {this.CurrentModuleId}");
                        }
                        // Jeśli nie, sprawdź ustawienia aplikacji
                        else
                        {
                            int savedModuleId = await Helpers.SettingsHelper.GetCurrentModuleIdAsync();
                            if (savedModuleId > 0 && modules.Any(m => m.ModuleId == savedModuleId))
                            {
                                this.CurrentModuleId = savedModuleId;
                                System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Ustawiono moduł z ustawień: {this.CurrentModuleId}");
                            }
                            // Jeśli nadal nie mamy bieżącego modułu, użyj pierwszego z listy
                            else if (modules.Count > 0)
                            {
                                this.CurrentModuleId = modules[0].ModuleId;
                                System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Ustawiono pierwszy moduł z listy: {this.CurrentModuleId}");
                            }
                        }
                    }

                    // Pobierz bieżący moduł
                    if (this.CurrentModuleId > 0)
                    {
                        // Ustaw bieżący moduł w serwisie i pobierz go
                        await this.specializationService.SetCurrentModuleAsync(this.CurrentModuleId);
                        this.CurrentModule = await this.specializationService.GetCurrentModuleAsync();
                        System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Ustawiono bieżący moduł: {this.CurrentModule?.Name ?? "null"}");

                        // Ustaw stan wyboru modułu
                        if (this.CurrentModule != null)
                        {
                            this.BasicModuleSelected = this.CurrentModule.Type == ModuleType.Basic;
                            this.SpecialisticModuleSelected = this.CurrentModule.Type == ModuleType.Specialistic;
                            System.Diagnostics.Debug.WriteLine($"DashboardViewModel: Typ modułu: {this.CurrentModule.Type}, Basic: {this.BasicModuleSelected}, Spec: {this.SpecialisticModuleSelected}");
                        }
                    }
                }
                else
                {
                    // Dla specjalizacji bez modułów, wyczyść stan modułów
                    this.CurrentModule = null;
                    this.CurrentModuleId = 0;
                    this.BasicModuleSelected = false;
                    this.SpecialisticModuleSelected = false;
                    this.AvailableModules.Clear();
                    System.Diagnostics.Debug.WriteLine("DashboardViewModel: Specjalizacja bez modułów, wyczyszczono stan modułów");
                }

                // Załaduj statystyki i aktualizuj UI
                await this.LoadStatisticsAsync();
                this.UpdateUIText();
                System.Diagnostics.Debug.WriteLine("DashboardViewModel: Zakończono ładowanie danych");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania danych dashboard: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania danych. Spróbuj ponownie później.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                // Określ ID aktualnie wybranego modułu
                int? moduleId = this.CurrentModuleId;

                // Pobierz ogólny postęp
                this.OverallProgress = await Helpers.ProgressCalculator.GetOverallProgressAsync(
                    this.databaseService,
                    this.CurrentSpecialization.SpecializationId,
                    moduleId);

                // Staże
                int completedInternships = await this.specializationService.GetInternshipCountAsync(moduleId);
                int totalInternships = 0;

                if (this.CurrentModule != null)
                {
                    totalInternships = this.CurrentModule.TotalInternships;
                }

                this.InternshipCount = $"{completedInternships}/{totalInternships}";
                this.InternshipProgress = totalInternships > 0
                    ? (double)completedInternships / totalInternships
                    : 0;

                // Procedury (skupiamy się na procedurach typu A zgodnie z JSONem)
                int completedProceduresA = await this.specializationService.GetProcedureCountAsync(moduleId);
                int totalProceduresA = 0;

                if (this.CurrentModule != null)
                {
                    totalProceduresA = this.CurrentModule.TotalProceduresA;
                }

                this.ProcedureCount = $"{completedProceduresA}/{totalProceduresA}";
                this.ProcedureProgress = totalProceduresA > 0
                    ? (double)completedProceduresA / totalProceduresA
                    : 0;

                // Kursy
                int completedCourses = await this.specializationService.GetCourseCountAsync(moduleId);
                int totalCourses = 0;

                if (this.CurrentModule != null)
                {
                    totalCourses = this.CurrentModule.TotalCourses;
                }

                this.CourseCount = $"{completedCourses}/{totalCourses}";
                this.CourseProgress = totalCourses > 0
                    ? (double)completedCourses / totalCourses
                    : 0;

                // Dyżury - POPRAWKA: Przekazujemy moduleId do GetSpecializationStatisticsAsync
                int completedShiftHours = await this.specializationService.GetShiftCountAsync(moduleId);

                // Pobierz pełne statystyki dla WYBRANEGO MODUŁU, a nie całej specjalizacji
                SpecializationStatistics stats = await this.specializationService.GetSpecializationStatisticsAsync(moduleId);

                if (stats.RequiredShiftHours > 0)
                {
                    this.ShiftStats = $"{completedShiftHours}/{stats.RequiredShiftHours}h";
                    this.ShiftProgress = Math.Min(1.0, (double)completedShiftHours / stats.RequiredShiftHours);
                }
                else
                {
                    this.ShiftStats = $"{completedShiftHours}h";
                    this.ShiftProgress = 0;
                }

                // Pozostałe liczniki
                this.SelfEducationCount = await this.specializationService.GetSelfEducationCountAsync(moduleId);
                this.PublicationCount = await this.specializationService.GetPublicationCountAsync(moduleId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania statystyk: {ex.Message}");
                // Nie wyświetlamy błędu użytkownikowi - statystyki są elementem pomocniczym
            }
        }

        private void UpdateUIText()
        {
            if (this.CurrentSpecialization == null)
            {
                return;
            }

            // Ustaw tytuł na podstawie bieżącego modułu
            if (this.HasModules && this.CurrentModule != null)
            {
                this.ModuleTitle = this.CurrentModule.Name;
            }
            else
            {
                this.ModuleTitle = this.CurrentSpecialization.Name;
            }

            // Ustaw informacje o specjalizacji
            this.SpecializationInfo = $"{this.CurrentSpecialization.Name}";

            // Ustaw zakres dat
            string startDate = this.CurrentSpecialization.StartDate.ToString("dd-MM-yyyy");
            string endDate = this.CurrentSpecialization.CalculatedEndDate.ToString("dd-MM-yyyy");
            this.DateRangeInfo = $"{startDate} - {endDate}";

            // Ustaw tekst postępu
            int progressPercent = (int)(this.OverallProgress * 100);
            this.ProgressText = $"Ukończono {progressPercent}%";
        }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            if (this.CurrentSpecialization == null || !this.HasModules)
            {
                return;
            }

            try
            {
                var modules = await this.databaseService.GetModulesAsync(this.CurrentSpecialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        this.CurrentModuleId = basicModule.ModuleId;
                        this.BasicModuleSelected = true;
                        this.SpecialisticModuleSelected = false;
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        this.CurrentModuleId = specialisticModule.ModuleId;
                        this.BasicModuleSelected = false;
                        this.SpecialisticModuleSelected = true;
                    }
                }

                // Zapisz wybrany moduł w ustawieniach
                await Helpers.SettingsHelper.SetCurrentModuleIdAsync(this.CurrentModuleId);

                // Ustaw bieżący moduł w specjalizacji
                if (this.CurrentSpecialization.CurrentModuleId != this.CurrentModuleId)
                {
                    this.CurrentSpecialization.CurrentModuleId = this.CurrentModuleId;
                    await this.databaseService.UpdateSpecializationAsync(this.CurrentSpecialization);
                }

                // Odśwież dane
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zmiany modułu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas przełączania modułu. Spróbuj ponownie.",
                    "OK");
            }
        }

        // Metody nawigacji
        private async Task NavigateToInternshipsAsync()
        {
            await Shell.Current.GoToAsync("Internships");
        }

        private async Task NavigateToProceduresAsync()
        {
            await Shell.Current.GoToAsync("Procedures");
        }

        private async Task NavigateToShiftsAsync()
        {
            await Shell.Current.GoToAsync("MedicalShifts");
        }

        private async Task NavigateToCoursesAsync()
        {
            await Shell.Current.GoToAsync("Courses");
        }

        private async Task NavigateToSelfEducationAsync()
        {
            await Shell.Current.GoToAsync("SelfEducation");
        }

        private async Task NavigateToPublicationsAsync()
        {
            await Shell.Current.GoToAsync("Publications");
        }

        private async Task NavigateToAbsencesAsync()
        {
            await Shell.Current.GoToAsync("Absences");
        }

        private async Task NavigateToStatisticsAsync()
        {
            await Shell.Current.GoToAsync("Statistics");
        }

        private async Task NavigateToExportAsync()
        {
            await Shell.Current.GoToAsync("Export");
        }

        private async Task NavigateToRecognitionsAsync()
        {
            // Uznania są tylko dla modułu specjalistycznego
            if (this.HasModules && !this.SpecialisticModuleSelected)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Informacja",
                    "Uznania i skrócenia są dostępne tylko dla modułu specjalistycznego. Przełącz się na moduł specjalistyczny, aby uzyskać dostęp do tej funkcji.",
                    "OK");
                return;
            }

            await Shell.Current.GoToAsync("Recognitions");
        }
    }
}