using CommunityToolkit.Mvvm.ComponentModel;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;

namespace SledzSpecke.App.ViewModels.Statistics
{
    public partial class SpecializationStatsViewModel : BaseViewModel
    {
        private readonly ISpecializationService _specializationService;
        private readonly IProcedureService _procedureService;
        private readonly IDutyService _dutyService;
        private readonly ICourseService _courseService;
        private readonly IInternshipService _internshipService;

        public SpecializationStatsViewModel(
            ISpecializationService specializationService,
            IProcedureService procedureService,
            IDutyService dutyService,
            ICourseService courseService,
            IInternshipService internshipService)
        {
            _specializationService = specializationService;
            _procedureService = procedureService;
            _dutyService = dutyService;
            _courseService = courseService;
            _internshipService = internshipService;
            
            Title = "Statystyki specjalizacji";
        }

        [ObservableProperty]
        private double overallProgress;

        [ObservableProperty]
        private string progressText;

        [ObservableProperty]
        private string timeLeftText;

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
        private Specialization currentSpecialization;

        [ObservableProperty]
        private SpecializationProgress specializationProgress;

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Pobierz bieżącą specjalizację
                CurrentSpecialization = await _specializationService.GetCurrentSpecializationAsync();
                if (CurrentSpecialization == null)
                {
                    await Shell.Current.DisplayAlert(
                        "Brak specjalizacji",
                        "Nie wybrano żadnej specjalizacji. Dodaj specjalizację, aby zobaczyć statystyki.",
                        "OK");
                    return;
                }

                // Pobierz postępy
                SpecializationProgress = await _specializationService.GetProgressStatisticsAsync(CurrentSpecialization.Id);
                
                // Ustawienie wartości progresów
                ProceduresProgress = await _procedureService.GetProcedureCompletionPercentageAsync();
                
                var dutyStats = await _dutyService.GetDutyStatisticsAsync();
                var totalDutyHours = dutyStats.TotalHours;
                var remainingHours = dutyStats.RemainingHours > 0 ? dutyStats.RemainingHours : 1;
                DutiesProgress = Math.Min(1.0, (double)(totalDutyHours / (totalDutyHours + remainingHours)));
                
                CoursesProgress = await _courseService.GetCourseProgressAsync();
                InternshipsProgress = await _internshipService.GetInternshipProgressAsync();
                
                // Obliczenie ogólnego postępu
                OverallProgress = (ProceduresProgress + DutiesProgress + CoursesProgress + InternshipsProgress) / 4.0;
                
                // Ustawienie tekstów
                ProgressText = $"{OverallProgress:P0} ukończone";
                ProceduresProgressText = $"{ProceduresProgress:P0} ukończone";
                DutiesProgressText = $"{DutiesProgress:P0} ukończone ({totalDutyHours:F1}h z {totalDutyHours + remainingHours:F1}h)";
                CoursesProgressText = $"{CoursesProgress:P0} ukończone";
                InternshipsProgressText = $"{InternshipsProgress:P0} ukończone";
                
                // Obliczenie pozostałego czasu
                var daysLeft = CalculateRemainingDays();
                TimeLeftText = FormatTimeLeft(daysLeft);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Błąd",
                    $"Nie udało się załadować statystyk: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private int CalculateRemainingDays()
        {
            // Obliczenie dni pozostałych do końca specjalizacji
            var expectedEndDate = DateTime.Today.AddDays(CurrentSpecialization.DurationInWeeks * 7);
            return (expectedEndDate - DateTime.Today).Days;
        }

        private string FormatTimeLeft(int days)
        {
            if (days <= 0)
                return "Specjalizacja zakończona";
            if (days == 1)
                return "Pozostał 1 dzień";
            if (days < 30)
                return $"Pozostało {days} dni";
            if (days < 365)
            {
                int months = days / 30;
                return $"Pozostało około {months} {(months == 1 ? "miesiąc" : (months < 5 ? "miesiące" : "miesięcy"))}";
            }
            
            double years = days / 365.0;
            return $"Pozostało około {years:F1} {(years < 2 ? "rok" : (years < 5 ? "lata" : "lat"))}";
        }
    }
}
