using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Dashboard
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;

        // Current module selection
        private int currentModuleId;
        private Specialization currentSpecialization;
        private Module currentModule;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;

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

        public DashboardViewModel(ISpecializationService specializationService, IDatabaseService databaseService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;

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
        }

        // Properties
        public int CurrentModuleId
        {
            get => this.currentModuleId;
            set
            {
                this.SetProperty(ref this.currentModuleId, value);

                // Reload all data with new module filter
                this.LoadDataAsync();
            }
        }

        public bool HasModules => this.CurrentSpecialization?.HasModules ?? false;

        public Specialization CurrentSpecialization
        {
            get => this.currentSpecialization;
            set => this.SetProperty(ref this.currentSpecialization, value);
        }

        public Module CurrentModule
        {
            get => this.currentModule;
            set => this.SetProperty(ref this.currentModule, value);
        }

        public List<ModuleInfo> AvailableModules => this.CurrentSpecialization?.Modules?.Select(m => new ModuleInfo { Id = m.ModuleId, Name = m.Name }).ToList();

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

        // Methods
        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Load current specialization
                this.CurrentSpecialization = await this.specializationService.GetCurrentSpecializationAsync();

                if (this.CurrentSpecialization == null)
                {
                    return;
                }

                // If specialized has modules, get current module
                if (this.CurrentSpecialization.HasModules)
                {
                    if (this.CurrentModuleId == 0)
                    {
                        // Default to current module from specialization if not set
                        this.CurrentModuleId = this.CurrentSpecialization.CurrentModuleId ?? 0;
                    }

                    if (this.CurrentModuleId > 0)
                    {
                        // Set the current module in the service and then retrieve it
                        await this.specializationService.SetCurrentModuleAsync(this.CurrentModuleId);
                        this.CurrentModule = await this.specializationService.GetCurrentModuleAsync();

                        // Set module selection state
                        this.BasicModuleSelected = this.CurrentModule?.Type == Models.Enums.ModuleType.Basic;
                        this.SpecialisticModuleSelected = !this.BasicModuleSelected;
                    }
                    else
                    {
                        // Default to basic module
                        this.BasicModuleSelected = true;
                        this.SpecialisticModuleSelected = false;
                    }
                }

                // Load statistics
                await this.LoadStatisticsAsync();

                // Update UI text
                this.UpdateUIText();
            }
            catch (Exception ex)
            {
                // Handle exception
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
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
                // Load statistics based on current module or for whole specialization
                int? moduleId = this.CurrentSpecialization?.HasModules == true ? this.CurrentModuleId : null;

                // Get overall progress
                this.OverallProgress = await Helpers.ProgressCalculator.GetOverallProgressAsync(
                    this.databaseService,
                    this.CurrentSpecialization.SpecializationId,
                    moduleId);

                // Get counts
                int completedInternships = await this.specializationService.GetInternshipCountAsync(moduleId);
                int totalInternships = moduleId.HasValue && this.CurrentModule != null
                    ? this.CurrentModule.TotalInternships
                    : this.CurrentSpecialization.TotalInternships;

                this.InternshipCount = $"{completedInternships}/{totalInternships}";
                this.InternshipProgress = totalInternships > 0 ? (double)completedInternships / totalInternships : 0;

                int completedProcedures = await this.specializationService.GetProcedureCountAsync(moduleId);
                int totalProcedures = 0; // This would need to be calculated from requirements

                this.ProcedureCount = $"{completedProcedures}/{totalProcedures}";
                this.ProcedureProgress = totalProcedures > 0 ? (double)completedProcedures / totalProcedures : 0.5; // Default to 50% if no data

                int completedCourses = await this.specializationService.GetCourseCountAsync(moduleId);
                int totalCourses = moduleId.HasValue && this.CurrentModule != null
                    ? this.CurrentModule.TotalCourses
                    : this.CurrentSpecialization.TotalCourses;

                this.CourseCount = $"{completedCourses}/{totalCourses}";
                this.CourseProgress = totalCourses > 0 ? (double)completedCourses / totalCourses : 0;

                // Get shift statistics
                int completedShiftHours = await this.specializationService.GetShiftCountAsync(moduleId);
                this.ShiftStats = $"{completedShiftHours}h"; // This would need more context for total required hours
                this.ShiftProgress = 0.3; // Default to 30% if no data

                // Other counts
                this.SelfEducationCount = await this.specializationService.GetSelfEducationCountAsync(moduleId);
                this.PublicationCount = await this.specializationService.GetPublicationCountAsync(moduleId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading statistics: {ex.Message}");
            }
        }

        private void UpdateUIText()
        {
            if (this.CurrentSpecialization == null)
            {
                return;
            }

            // Set title based on current module
            if (this.CurrentSpecialization.HasModules && this.CurrentModule != null)
            {
                this.ModuleTitle = this.CurrentModule.Name;
            }
            else
            {
                this.ModuleTitle = "Specjalizacja";
            }

            // Set specialization info
            this.SpecializationInfo = $"{this.CurrentSpecialization.Name}";

            // Set date range
            string startDate = this.CurrentSpecialization.StartDate.ToString("d");
            string endDate = this.CurrentSpecialization.CalculatedEndDate.ToString("d");
            this.DateRangeInfo = $"{startDate} - {endDate}";

            // Set progress text
            int progressPercent = (int)(this.OverallProgress * 100);
            this.ProgressText = $"Ukończono {progressPercent}%";
        }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            if (this.CurrentSpecialization == null || !this.CurrentSpecialization.HasModules)
            {
                return;
            }

            try
            {
                var modules = await this.databaseService.GetModulesAsync(this.CurrentSpecialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == Models.Enums.ModuleType.Basic);
                    if (basicModule != null)
                    {
                        this.CurrentModuleId = basicModule.ModuleId;
                        this.BasicModuleSelected = true;
                        this.SpecialisticModuleSelected = false;
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == Models.Enums.ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        this.CurrentModuleId = specialisticModule.ModuleId;
                        this.BasicModuleSelected = false;
                        this.SpecialisticModuleSelected = true;
                    }
                }

                // Save selected module to settings
                await Helpers.Settings.SetCurrentModuleIdAsync(this.CurrentModuleId);

                // Reload data
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error selecting module: {ex.Message}");
            }
        }

        // Navigation methods
        private async Task NavigateToInternshipsAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToProceduresAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToShiftsAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToCoursesAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToSelfEducationAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToPublicationsAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToAbsencesAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToStatisticsAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToExportAsync()
        {
            // Navigation would be implemented here
        }

        private async Task NavigateToRecognitionsAsync()
        {
            // Navigation would be implemented here
        }
    }
}