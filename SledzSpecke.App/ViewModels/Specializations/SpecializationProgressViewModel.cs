using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Enums;
using System.Collections.ObjectModel;
using System.Text;

namespace SledzSpecke.App.ViewModels.Specializations
{
    public partial class SpecializationProgressViewModel : BaseViewModel
    {
        private readonly ISpecializationService _specializationService;
        private readonly IProcedureService _procedureService;
        private readonly ICourseService _courseService;
        private readonly IInternshipService _internshipService;
        private readonly IDutyService _dutyService;
        private readonly ISpecializationRequirementsProvider _requirementsProvider;

        public SpecializationProgressViewModel(
            ISpecializationService specializationService,
            IProcedureService procedureService,
            ICourseService courseService,
            IInternshipService internshipService,
            IDutyService dutyService,
            ISpecializationRequirementsProvider requirementsProvider)
        {
            _specializationService = specializationService;
            _procedureService = procedureService;
            _courseService = courseService;
            _internshipService = internshipService;
            _dutyService = dutyService;
            _requirementsProvider = requirementsProvider;

            Title = "Postęp specjalizacji";
            DetailedRequirements = new ObservableCollection<RequirementViewModel>();
            FilterOptions = new ObservableCollection<string>();
            SelectedTab = "Procedures";
        }

        [ObservableProperty]
        private string specializationName;

        [ObservableProperty]
        private double totalProgress;

        [ObservableProperty]
        private string totalProgressText;

        [ObservableProperty]
        private double proceduresProgress;

        [ObservableProperty]
        private string proceduresProgressText;

        [ObservableProperty]
        private double dutiesProgress;

        [ObservableProperty]
        private string dutiesProgressText;

        [ObservableProperty]
        private double coursesProgress;

        [ObservableProperty]
        private string coursesProgressText;

        [ObservableProperty]
        private double internshipsProgress;

        [ObservableProperty]
        private string internshipsProgressText;

        [ObservableProperty]
        private bool isDetailedViewVisible;

        [ObservableProperty]
        private string selectedTab;

        [ObservableProperty]
        private ObservableCollection<string> filterOptions;

        [ObservableProperty]
        private string selectedFilter;

        [ObservableProperty]
        private ObservableCollection<RequirementViewModel> detailedRequirements;

        public bool HasFilters => FilterOptions.Count > 0;

        public bool IsProceduresTabActive => SelectedTab == "Procedures";
        public bool IsDutiesTabActive => SelectedTab == "Duties";
        public bool IsCoursesTabActive => SelectedTab == "Courses";
        public bool IsInternshipsTabActive => SelectedTab == "Internships";

        [RelayCommand]
        private void ShowDetailedView()
        {
            IsDetailedViewVisible = true;
            LoadDetailedRequirements();
        }

        [RelayCommand]
        private void ShowSummaryView()
        {
            IsDetailedViewVisible = false;
        }

        [RelayCommand]
        private void SwitchTab(string tab)
        {
            if (SelectedTab == tab) return;

            SelectedTab = tab;
            OnPropertyChanged(nameof(IsProceduresTabActive));
            OnPropertyChanged(nameof(IsDutiesTabActive));
            OnPropertyChanged(nameof(IsCoursesTabActive));
            OnPropertyChanged(nameof(IsInternshipsTabActive));

            LoadDetailedRequirements();
        }

        [RelayCommand]
        private void ClearFilter()
        {
            SelectedFilter = null;
            LoadDetailedRequirements();
        }

        [RelayCommand]
        private void ToggleDetails(string id)
        {
            var requirement = DetailedRequirements.FirstOrDefault(r => r.Id == id);
            if (requirement != null)
            {
                requirement.HasDetails = !requirement.HasDetails;
            }
        }

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var currentSpecialization = await _specializationService.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    await Shell.Current.DisplayAlert("Błąd", "Nie wybrano specjalizacji", "OK");
                    return;
                }

                SpecializationName = currentSpecialization.Name;

                ProceduresProgress = await _procedureService.GetProcedureCompletionPercentageAsync();
                ProceduresProgressText = $"{ProceduresProgress:P0}";

                var dutyStats = await _dutyService.GetDutyStatisticsAsync();
                DutiesProgress = Math.Min(1.0, (double)(dutyStats.TotalHours / (dutyStats.TotalHours + dutyStats.RemainingHours)));
                DutiesProgressText = $"{DutiesProgress:P0}";

                CoursesProgress = await _courseService.GetCourseProgressAsync();
                CoursesProgressText = $"{CoursesProgress:P0}";

                InternshipsProgress = await _internshipService.GetInternshipProgressAsync();
                InternshipsProgressText = $"{InternshipsProgress:P0}";

                TotalProgress = (ProceduresProgress + DutiesProgress + CoursesProgress + InternshipsProgress) / 4.0;
                TotalProgressText = $"{TotalProgress:P0}";

                if (IsDetailedViewVisible)
                {
                    LoadDetailedRequirements();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void LoadDetailedRequirements()
        {
            try
            {
                DetailedRequirements.Clear();
                FilterOptions.Clear();

                var currentSpecialization = await _specializationService.GetCurrentSpecializationAsync();
                if (currentSpecialization == null) return;

                switch (SelectedTab)
                {
                    case "Procedures":
                        await LoadProceduresRequirements(currentSpecialization.Id);
                        break;
                    case "Duties":
                        await LoadDutiesRequirements(currentSpecialization.Id);
                        break;
                    case "Courses":
                        await LoadCoursesRequirements(currentSpecialization.Id);
                        break;
                    case "Internships":
                        await LoadInternshipsRequirements(currentSpecialization.Id);
                        break;
                }

                OnPropertyChanged(nameof(HasFilters));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania szczegółowych wymagań: {ex.Message}");
            }
        }

        private async Task LoadProceduresRequirements(int specializationId)
        {
            var procedureRequirements = _requirementsProvider.GetRequiredProceduresBySpecialization(specializationId);
            var categoryProgress = await _procedureService.GetProcedureProgressByCategoryAsync();

            foreach (var category in procedureRequirements.Keys)
            {
                FilterOptions.Add(category);
            }

            var filteredCategories = procedureRequirements.Keys.ToList();
            if (!string.IsNullOrEmpty(SelectedFilter) && procedureRequirements.ContainsKey(SelectedFilter))
            {
                filteredCategories = new List<string> { SelectedFilter };
            }

            foreach (var category in filteredCategories)
            {
                foreach (var procedure in procedureRequirements[category])
                {
                    double progress = 0;
                    string progressText = "0%";

                    if (categoryProgress.TryGetValue(category, out var progressData))
                    {
                        double requiredTotal = procedure.RequiredCount + procedure.AssistanceCount;
                        double completedTotal = 0;

                        if (requiredTotal > 0)
                        {
                            if (procedure.RequiredCount > 0)
                            {
                                completedTotal += Math.Min(progressData.Completed, procedure.RequiredCount);
                            }

                            if (procedure.AssistanceCount > 0)
                            {
                                completedTotal += Math.Min(progressData.Assisted, procedure.AssistanceCount);
                            }

                            progress = completedTotal / requiredTotal;
                            progressText = $"{progress:P0}";
                        }
                    }

                    var details = new StringBuilder();
                    if (procedure.RequiredCount > 0)
                    {
                        details.AppendLine($"Wymagane wykonania: {procedure.RequiredCount}");
                    }
                    if (procedure.AssistanceCount > 0)
                    {
                        details.AppendLine($"Wymagane asysty: {procedure.AssistanceCount}");
                    }

                    var requirement = new RequirementViewModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = procedure.Name,
                        Category = category,
                        Description = procedure.Description,
                        Details = details.ToString().TrimEnd(),
                        Progress = progress,
                        ProgressText = progressText,
                        HasDetails = false
                    };

                    DetailedRequirements.Add(requirement);
                }
            }
        }

        private async Task LoadDutiesRequirements(int specializationId)
        {
            var dutyRequirements = _requirementsProvider.GetDutyRequirementsBySpecialization(specializationId);
            var dutyStats = await _dutyService.GetDutyStatisticsAsync();
            var dutyTypes = dutyRequirements.Select(d => d.Type).Distinct().ToList();

            foreach (var type in dutyTypes)
            {
                FilterOptions.Add(type);
            }

            var filteredDutyTypes = dutyTypes;

            if (!string.IsNullOrEmpty(SelectedFilter))
            {
                filteredDutyTypes = dutyTypes.Where(t => t == SelectedFilter).ToList();
            }

            foreach (var dutyType in filteredDutyTypes)
            {
                var requirements = dutyRequirements.Where(d => d.Type == dutyType).ToList();

                foreach (var requirement in requirements)
                {
                    double progress = 0;
                    string progressText = "0%";

                    if (dutyStats.DutiesByType.TryGetValue((DutyType)Enum.Parse(typeof(DutyType), dutyType), out int count))
                    {
                        var monthlyRequired = requirement.MinimumDutiesPerMonth * 12 * requirement.Year;
                        progress = Math.Min(1.0, (double)count / monthlyRequired);
                        progressText = $"{progress:P0}";
                    }

                    var details = new StringBuilder();
                    details.AppendLine($"Rok: {requirement.Year}");
                    details.AppendLine($"Minimum miesięcznie: {requirement.MinimumDutiesPerMonth} dyżurów");
                    details.AppendLine($"Minimum godzin miesięcznie: {requirement.MinimumHoursPerMonth}");

                    if (requirement.RequiresSupervision)
                    {
                        details.AppendLine("Wymagany nadzór: Tak");
                    }

                    if (requirement.RequiredCompetencies.Any())
                    {
                        details.AppendLine("Wymagane kompetencje:");
                        foreach (var competency in requirement.RequiredCompetencies)
                        {
                            details.AppendLine($"- {competency}");
                        }
                    }

                    var requirementViewModel = new RequirementViewModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"Dyżur typu {dutyType}",
                        Category = "Dyżury",
                        Description = $"Wymogi dla dyżurów typu {dutyType}",
                        Details = details.ToString().TrimEnd(),
                        Progress = progress,
                        ProgressText = progressText,
                        HasDetails = false
                    };

                    DetailedRequirements.Add(requirementViewModel);
                }
            }
        }

        private async Task LoadCoursesRequirements(int specializationId)
        {
            var requiredCourses = await _courseService.GetRequiredCoursesAsync();
            var userCourses = await _courseService.GetUserCoursesAsync();
            var yearProgress = await _courseService.GetCourseProgressByYearAsync();
            var years = requiredCourses.Select(c => c.RecommendedYear).Distinct().OrderBy(y => y).ToList();

            foreach (var year in years)
            {
                FilterOptions.Add($"Rok {year}");
            }

            var filteredYears = years;
            if (!string.IsNullOrEmpty(SelectedFilter) && SelectedFilter.StartsWith("Rok "))
            {
                int.TryParse(SelectedFilter.Replace("Rok ", ""), out int selectedYear);
                filteredYears = years.Where(y => y == selectedYear).ToList();
            }

            foreach (var year in filteredYears)
            {
                var coursesForYear = requiredCourses.Where(c => c.RecommendedYear == year).ToList();

                foreach (var course in coursesForYear)
                {
                    bool isCompleted = userCourses.Any(c => c.CourseDefinitionId == course.Id && c.IsCompleted);
                    double progress = isCompleted ? 1.0 : 0.0;
                    string progressText = isCompleted ? "100%" : "0%";

                    var details = new StringBuilder();
                    details.AppendLine($"Rok: {course.RecommendedYear}");
                    details.AppendLine($"Czas trwania: {course.DurationInHours} godz. ({course.DurationInDays} dni)");

                    if (course.IsRequired)
                    {
                        details.AppendLine("Obowiązkowy: Tak");
                    }
                    else
                    {
                        details.AppendLine("Obowiązkowy: Nie");
                    }

                    if (course.CanBeRemote)
                    {
                        details.AppendLine("Może być zdalny: Tak");
                    }

                    var completionRequirements = string.Empty;
                    if (!string.IsNullOrEmpty(course.CompletionRequirements))
                    {
                        completionRequirements = $"Wymagania zaliczenia: {course.CompletionRequirements}";
                    }

                    var requirementViewModel = new RequirementViewModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = course.Name,
                        Category = $"Rok {year}",
                        Description = course.Description,
                        Details = details.ToString().TrimEnd(),
                        CompletionRequirements = completionRequirements,
                        Progress = progress,
                        ProgressText = progressText,
                        HasDetails = false
                    };

                    DetailedRequirements.Add(requirementViewModel);
                }
            }
        }

        private async Task LoadInternshipsRequirements(int specializationId)
        {
            var requiredInternships = await _internshipService.GetRequiredInternshipsAsync();
            var userInternships = await _internshipService.GetUserInternshipsAsync();
            var yearProgress = await _internshipService.GetInternshipProgressByYearAsync();
            var years = requiredInternships.Select(i => i.RecommendedYear).Distinct().OrderBy(y => y).ToList();

            foreach (var year in years)
            {
                FilterOptions.Add($"Rok {year}");
            }

            var filteredYears = years;
            if (!string.IsNullOrEmpty(SelectedFilter) && SelectedFilter.StartsWith("Rok "))
            {
                int.TryParse(SelectedFilter.Replace("Rok ", ""), out int selectedYear);
                filteredYears = years.Where(y => y == selectedYear).ToList();
            }

            foreach (var year in filteredYears)
            {
                var internshipsForYear = requiredInternships.Where(i => i.RecommendedYear == year).ToList();

                foreach (var internship in internshipsForYear)
                {
                    bool isCompleted = userInternships.Any(i => i.InternshipDefinitionId == internship.Id && i.IsCompleted);
                    double progress = isCompleted ? 1.0 : 0.0;
                    string progressText = isCompleted ? "100%" : "0%";
                    var details = new StringBuilder();

                    details.AppendLine($"Rok: {internship.RecommendedYear}");
                    details.AppendLine($"Czas trwania: {internship.DurationInWeeks} tygodni");

                    if (internship.IsRequired)
                    {
                        details.AppendLine("Obowiązkowy: Tak");
                    }
                    else
                    {
                        details.AppendLine("Obowiązkowy: Nie");
                    }

                    try
                    {
                        var modules = await _internshipService.GetModulesForInternshipAsync(internship.Id);
                        if (modules != null && modules.Any())
                        {
                            details.AppendLine("\nModuły stażu:");
                            foreach (var module in modules)
                            {
                                details.AppendLine($"- {module.Name} ({module.DurationInWeeks} tygodni)");
                            }
                        }
                    }
                    catch
                    {
                    }

                    var completionRequirements = string.Empty;
                    if (internship.CompletionRequirements != null && internship.CompletionRequirements.Any())
                    {
                        completionRequirements = $"Wymagania zaliczenia: {string.Join(", ", internship.CompletionRequirements)}";
                    }

                    var requirementViewModel = new RequirementViewModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = internship.Name,
                        Category = $"Rok {year}",
                        Description = internship.Description,
                        Details = details.ToString().TrimEnd(),
                        CompletionRequirements = completionRequirements,
                        Progress = progress,
                        ProgressText = progressText,
                        HasDetails = false
                    };

                    DetailedRequirements.Add(requirementViewModel);
                }
            }
        }

        public partial class RequirementViewModel : ObservableObject
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string Details { get; set; }
            public string CompletionRequirements { get; set; }
            public double Progress { get; set; }
            public string ProgressText { get; set; }

            [ObservableProperty]
            private bool hasDetails;

            public string ToggleDetailsText => HasDetails ? "Ukryj szczegóły" : "Pokaż szczegóły";
        }
    }
}