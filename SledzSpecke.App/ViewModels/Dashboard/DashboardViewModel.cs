using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Dashboard
{
    public class DashboardViewModel : BaseViewModel, IDisposable
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;
        private readonly IProcedureService procedureService;

        private int currentModuleId;
        private Models.Specialization currentSpecialization;
        private Module currentModule;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private bool hasTwoModules;
        private ObservableCollection<ModuleInfo> availableModules;
        private double overallProgress;
        private double internshipProgress;
        private double courseProgress;
        private double procedureProgress;
        private double shiftProgress;
        private string internshipCount;
        private string procedureCount;
        private string courseCount;
        private string shiftStats;
        private int selfEducationCount;
        private int publicationCount;
        private string moduleTitle;
        private string specializationInfo;
        private string dateRangeInfo;
        private string progressText;

        public DashboardViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService,
            IProcedureService procedureService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;
            this.procedureService = procedureService;

            this.AvailableModules = new ObservableCollection<ModuleInfo>();

            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);
            this.NavigateToInternshipsCommand = new AsyncRelayCommand(NavigateToInternshipsAsync);
            this.NavigateToProceduresCommand = new AsyncRelayCommand(NavigateToProceduresAsync);
            this.NavigateToShiftsCommand = new AsyncRelayCommand(NavigateToShiftsAsync);
            this.NavigateToCoursesCommand = new AsyncRelayCommand(NavigateToCoursesAsync);
            this.NavigateToSelfEducationCommand = new AsyncRelayCommand(NavigateToSelfEducationAsync);
            this.NavigateToPublicationsCommand = new AsyncRelayCommand(NavigateToPublicationsAsync);
            this.NavigateToAbsencesCommand = new AsyncRelayCommand(NavigateToAbsencesAsync);
            this.NavigateToStatisticsCommand = new AsyncRelayCommand(NavigateToStatisticsAsync);
            this.NavigateToExportCommand = new AsyncRelayCommand(NavigateToExportAsync);
            this.NavigateToRecognitionsCommand = new AsyncRelayCommand(NavigateToRecognitionsAsync);

            this.specializationService.CurrentModuleChanged += this.OnModuleChanged;

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public int CurrentModuleId
        {
            get => this.currentModuleId;
            set
            {
                if (this.SetProperty(ref this.currentModuleId, value))
                {
                    this.LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
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

        private async void OnModuleChanged(object sender, int moduleId)
        {
            this.CurrentModuleId = moduleId;

            await SafeExecuteAsync(async () =>
            {
                var module = await this.databaseService.GetModuleAsync(moduleId);
                await this.LoadDataAsync();
            }, "Wystąpił problem podczas zmiany modułu.");
        }

        public void Dispose()
        {
            this.specializationService.CurrentModuleChanged -= this.OnModuleChanged;
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                this.CurrentSpecialization = await this.specializationService.GetCurrentSpecializationAsync();

                if (this.CurrentSpecialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Active specialization not found",
                        "Nie znaleziono aktywnej specjalizacji. Proszę skontaktować się z administratorem.");
                }

                this.HasTwoModules = this.CurrentSpecialization.Modules.Any(x => x.Type == ModuleType.Basic);
                await this.specializationService.InitializeSpecializationModulesAsync(this.CurrentSpecialization.SpecializationId);
                var modules = await this.databaseService.GetModulesAsync(this.CurrentSpecialization.SpecializationId);
                this.AvailableModules.Clear();

                foreach (var module in modules)
                {
                    this.AvailableModules.Add(new ModuleInfo
                    {
                        Id = module.ModuleId,
                        Name = module.Name,
                    });
                }

                if (this.CurrentModuleId == 0)
                {
                    if (this.CurrentSpecialization.CurrentModuleId.HasValue && this.CurrentSpecialization.CurrentModuleId.Value > 0)
                    {
                        this.CurrentModuleId = this.CurrentSpecialization.CurrentModuleId.Value;
                    }
                    else
                    {
                        int savedModuleId = await Helpers.SettingsHelper.GetCurrentModuleIdAsync();
                        if (savedModuleId > 0 && modules.Any(m => m.ModuleId == savedModuleId))
                        {
                            this.CurrentModuleId = savedModuleId;
                        }
                        else if (modules.Count > 0)
                        {
                            this.CurrentModuleId = modules[0].ModuleId;
                        }
                    }
                }

                if (this.CurrentModuleId > 0)
                {
                    await this.specializationService.SetCurrentModuleAsync(this.CurrentModuleId);
                    this.CurrentModule = await this.specializationService.GetCurrentModuleAsync();

                    if (this.CurrentModule != null)
                    {
                        this.BasicModuleSelected = this.CurrentModule.Type == ModuleType.Basic;
                        this.SpecialisticModuleSelected = this.CurrentModule.Type == ModuleType.Specialistic;
                    }
                }

                await this.LoadStatisticsAsync();
                this.UpdateUIText();
            }, "Wystąpił problem podczas ładowania danych. Spróbuj ponownie.");

            this.IsBusy = false;
        }

        private async Task LoadStatisticsAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                int? moduleId = this.CurrentModuleId;
                this.OverallProgress = await Helpers.ProgressCalculator.GetOverallProgressAsync(
                    this.databaseService,
                    this.CurrentSpecialization.SpecializationId,
                    moduleId);
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
                var procedureStats = await this.procedureService.GetProcedureStatisticsForModuleAsync(this.CurrentModuleId);
                int completedProcedures = procedureStats.completed;
                int totalProcedures = procedureStats.total;

                this.ProcedureCount = $"{completedProcedures}/{totalProcedures}";
                this.ProcedureProgress = totalProcedures > 0
                    ? (double)completedProcedures / totalProcedures
                    : 0;

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

                int completedShiftHours = await this.specializationService.GetShiftCountAsync(moduleId);
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

                this.SelfEducationCount = await this.specializationService.GetSelfEducationCountAsync(moduleId);
                this.PublicationCount = await this.specializationService.GetPublicationCountAsync(moduleId);

                if (this.CurrentModule != null)
                {
                    double internshipWeight = 0.35;
                    double courseWeight = 0.25;
                    double procedureWeight = 0.30;
                    double otherWeight = 0.10;

                    this.OverallProgress =
                        (this.InternshipProgress * internshipWeight) +
                        (this.CourseProgress * courseWeight) +
                        (this.ProcedureProgress * procedureWeight) +
                        (this.ShiftProgress * otherWeight);

                    this.OverallProgress = Math.Min(1.0, this.OverallProgress);
                }
            }, "Wystąpił problem podczas obliczania statystyk.");
        }

        private void UpdateUIText()
        {
            if (this.CurrentSpecialization == null)
            {
                return;
            }

            if (this.CurrentModule != null)
            {
                this.ModuleTitle = this.CurrentModule.Name;
            }
            else
            {
                this.ModuleTitle = this.CurrentSpecialization.Name;
            }

            this.SpecializationInfo = $"{this.CurrentSpecialization.Name}";
            string startDate = this.CurrentSpecialization.StartDate.ToString("dd-MM-yyyy");
            string endDate = this.CurrentSpecialization.CalculatedEndDate.ToString("dd-MM-yyyy");
            this.DateRangeInfo = $"{startDate} - {endDate}";
            int progressPercent = (int)(this.OverallProgress * 100);
            this.ProgressText = $"Ukończono {progressPercent}%";
        }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            if (this.CurrentSpecialization == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
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

                await Helpers.SettingsHelper.SetCurrentModuleIdAsync(this.CurrentModuleId);

                if (this.CurrentSpecialization.CurrentModuleId != this.CurrentModuleId)
                {
                    this.CurrentSpecialization.CurrentModuleId = this.CurrentModuleId;
                    await this.databaseService.UpdateSpecializationAsync(this.CurrentSpecialization);
                }

                await this.LoadDataAsync();
            }, "Nie udało się przełączyć modułu. Spróbuj ponownie.");
        }

        private static async Task NavigateToInternshipsAsync()
        {
            await Shell.Current.GoToAsync("/internships");
        }

        private static async Task NavigateToProceduresAsync()
        {
            await Shell.Current.GoToAsync("/ProcedureSelector");
        }

        private static async Task NavigateToShiftsAsync()
        {
            await Shell.Current.GoToAsync("///medicalshifts");
        }

        private static async Task NavigateToCoursesAsync()
        {
            await Shell.Current.GoToAsync("courses");
        }

        private static async Task NavigateToSelfEducationAsync()
        {
            await Shell.Current.GoToAsync("selfeducation");
        }

        private static async Task NavigateToPublicationsAsync()
        {
            await Shell.Current.GoToAsync("publications");
        }

        private static async Task NavigateToAbsencesAsync()
        {
            await Shell.Current.GoToAsync("absences");
        }

        private static async Task NavigateToStatisticsAsync()
        {
            await Shell.Current.GoToAsync("statistics");
        }

        private static async Task NavigateToExportAsync()
        {
            await Shell.Current.GoToAsync("export");
        }

        private async Task NavigateToRecognitionsAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                if (!this.SpecialisticModuleSelected)
                {
                    throw new BusinessRuleViolationException(
                        "Recognitions only available in specialist module",
                        "Uznania i skrócenia są dostępne tylko dla modułu specjalistycznego. Przełącz się na moduł specjalistyczny, aby uzyskać dostęp do tej funkcji.");
                }

                await Shell.Current.GoToAsync("Recognitions");
            }, "Wystąpił problem podczas przechodzenia do uznań i skróceń.");
        }
    }
}