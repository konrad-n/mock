using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.ViewModels.Dashboard
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IProcedureService _procedureService;
        private readonly ICourseService _courseService;
        private readonly IInternshipService _internshipService;
        private readonly IDutyService _dutyService;
        private readonly IUserService _userService;

        public DashboardViewModel(
            IProcedureService procedureService,
            ICourseService courseService,
            IInternshipService internshipService,
            IDutyService dutyService,
            IUserService userService)
        {
            _procedureService = procedureService;
            _courseService = courseService;
            _internshipService = internshipService;
            _dutyService = dutyService;
            _userService = userService;

            Title = "Dashboard";
            RecommendedCourses = new ObservableCollection<CourseDefinition>();
            RecommendedInternships = new ObservableCollection<InternshipDefinition>();
        }

        [ObservableProperty]
        private double proceduresProgress;

        [ObservableProperty]
        private string proceduresProgressText;

        [ObservableProperty]
        private double coursesProgress;

        [ObservableProperty]
        private string coursesProgressText;

        [ObservableProperty]
        private double internshipsProgress;

        [ObservableProperty]
        private string internshipsProgressText;

        [ObservableProperty]
        private double dutiesProgress;

        [ObservableProperty]
        private string dutiesProgressText;

        [ObservableProperty]
        private double overallProgress;

        [ObservableProperty]
        private string overallProgressText;

        [ObservableProperty]
        private string timeLeftText;

        [ObservableProperty]
        private ObservableCollection<CourseDefinition> recommendedCourses;

        [ObservableProperty]
        private ObservableCollection<InternshipDefinition> recommendedInternships;

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Calculate progress
                ProceduresProgress = await _procedureService.GetProcedureCompletionPercentageAsync();
                ProceduresProgressText = $"{ProceduresProgress:P0}";

                CoursesProgress = await _courseService.GetCourseProgressAsync();
                CoursesProgressText = $"{CoursesProgress:P0}";

                InternshipsProgress = await _internshipService.GetInternshipProgressAsync();
                InternshipsProgressText = $"{InternshipsProgress:P0}";

                var dutyStats = await _dutyService.GetDutyStatisticsAsync();

                // Fix: Convert decimal to double for division
                double totalHours = (double)dutyStats.TotalHours;
                double remainingHours = (double)(dutyStats.RemainingHours > 0 ? dutyStats.RemainingHours : 1);

                DutiesProgress = totalHours / (totalHours + remainingHours);
                DutiesProgressText = $"{dutyStats.TotalHours}/{dutyStats.TotalHours + dutyStats.RemainingHours}h";

                // Calculate overall progress
                OverallProgress = (ProceduresProgress + CoursesProgress + InternshipsProgress + DutiesProgress) / 4;
                OverallProgressText = $"{OverallProgress:P0} ukończone";

                // Calculate remaining time
                var user = await _userService.GetCurrentUserAsync();
                if (user != null)
                {
                    var daysLeft = (user.ExpectedEndDate - DateTime.Today).Days;
                    TimeLeftText = daysLeft switch
                    {
                        < 0 => "Specjalizacja zakończona",
                        0 => "Ostatni dzień specjalizacji",
                        1 => "Pozostał 1 dzień",
                        _ => $"Pozostało {daysLeft} dni"
                    };
                }
                else
                {
                    TimeLeftText = "Brak danych";
                }

                // For testing purposes, we can leave these empty for now
                RecommendedCourses.Clear();
                RecommendedInternships.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                // Don't show alert to user in this stage
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToProceduresAsync()
        {
            await Shell.Current.GoToAsync("//procedures");
        }

        [RelayCommand]
        private async Task NavigateToCoursesAsync()
        {
            await Shell.Current.GoToAsync("//courses");
        }

        [RelayCommand]
        private async Task NavigateToInternshipsAsync()
        {
            await Shell.Current.GoToAsync("//internships");
        }

        [RelayCommand]
        private async Task NavigateToDutiesAsync()
        {
            await Shell.Current.GoToAsync("//duties");
        }
    }
}
