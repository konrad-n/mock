using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Dashboard.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly ISpecializationService _specializationService;
        private readonly ISpecializationDateCalculator _specializationDateCalculator;
        private readonly IDutyShiftService _dutyShiftService;
        private readonly ISelfEducationService _selfEducationService;
        private Specialization _specialization;

        [ObservableProperty]
        private string _startDateLabel;

        [ObservableProperty]
        private string _plannedEndDateLabel;

        [ObservableProperty]
        private string _actualEndDateLabel;

        [ObservableProperty]
        private string _daysLeftLabel;

        [ObservableProperty]
        private string _currentStageLabel;

        [ObservableProperty]
        private double _totalProgressBarValue;

        [ObservableProperty]
        private string _totalProgressLabel;

        [ObservableProperty]
        private double _basicModuleProgressBarValue;

        [ObservableProperty]
        private string _basicModuleProgressLabel;

        [ObservableProperty]
        private double _specialisticModuleProgressBarValue;

        [ObservableProperty]
        private string _specialisticModuleProgressLabel;

        [ObservableProperty]
        private string _coursesLabel;

        [ObservableProperty]
        private string _internshipsLabel;

        [ObservableProperty]
        private string _proceduresALabel;

        [ObservableProperty]
        private string _proceduresBLabel;

        [ObservableProperty]
        private string _dutyShiftsLabel;

        [ObservableProperty]
        private string _selfEducationLabel;

        [ObservableProperty]
        private string _upcomingEvent1;

        [ObservableProperty]
        private string _upcomingEvent2;

        [ObservableProperty]
        private string _upcomingEvent3;

        [ObservableProperty]
        private bool _upcomingEvent2Visible;

        [ObservableProperty]
        private bool _upcomingEvent3Visible;

        [ObservableProperty]
        private Color _upcomingEvent1Color;

        [ObservableProperty]
        private Color _upcomingEvent2Color;

        [ObservableProperty]
        private Color _upcomingEvent3Color;

        public DashboardViewModel(
            ISpecializationService specializationService,
            ISpecializationDateCalculator specializationDateCalculator,
            IDutyShiftService dutyShiftService,
            ISelfEducationService selfEducationService,
            ILogger<DashboardViewModel> logger) : base(logger)
        {
            _specializationService = specializationService;
            _specializationDateCalculator = specializationDateCalculator;
            _dutyShiftService = dutyShiftService;
            _selfEducationService = selfEducationService;

            Title = "Dashboard";
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                await LoadSpecializationDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadSpecializationDataAsync()
        {
            _specialization = await _specializationService.GetSpecializationAsync();

            await LoadDashboardDataAsync();
        }

        private async Task LoadDashboardDataAsync()
        {
            // Podstawowe informacje o specjalizacji
            StartDateLabel = _specialization.StartDate.ToString("dd-MM-yyyy");

            // Oblicz datę zakończenia bez nieobecności (zgodnie z programem)
            DateTime plannedEndDate = _specialization.StartDate.AddDays(_specialization.BaseDurationWeeks * 7);
            PlannedEndDateLabel = plannedEndDate.ToString("dd-MM-yyyy");

            // Inicjalizacja domyślną wartością
            DateTime actualEndDate = plannedEndDate;
            try
            {
                actualEndDate = await _specializationDateCalculator.CalculateExpectedEndDateAsync(_specialization.Id);
                ActualEndDateLabel = actualEndDate.ToString("dd-MM-yyyy");
            }
            catch (Exception ex)
            {
                // W przypadku błędu, pokazujemy planowaną datę
                ActualEndDateLabel = plannedEndDate.ToString("dd-MM-yyyy");
                _logger.LogError(ex, "Error calculating actual end date");
            }

            // Oblicz pozostałe dni, używając daty z nieobecnościami
            var daysLeft = (actualEndDate - DateTime.Now).Days;
            DaysLeftLabel = daysLeft > 0 ? daysLeft.ToString() : "0";

            // Określenie obecnego etapu
            var daysSinceStart = (DateTime.Now - _specialization.StartDate).Days;
            if (daysSinceStart < (_specialization.BasicModuleDurationWeeks * 7))
            {
                CurrentStageLabel = "Moduł podstawowy";
            }
            else
            {
                CurrentStageLabel = "Moduł specjalistyczny";
            }

            // Ogólny postęp
            var totalProgress = _specialization.GetCompletionPercentage() / 100;
            TotalProgressBarValue = totalProgress;
            TotalProgressLabel = $"{(totalProgress * 100):F0}% ukończono";

            // Postęp modułów
            var basicModuleProgress = GetBasicModuleProgress();
            BasicModuleProgressBarValue = basicModuleProgress;
            BasicModuleProgressLabel = $"{(basicModuleProgress * 100):F0}%";

            var specialisticModuleProgress = GetSpecialisticModuleProgress();
            SpecialisticModuleProgressBarValue = specialisticModuleProgress;
            SpecialisticModuleProgressLabel = $"{(specialisticModuleProgress * 100):F0}%";

            // Statystyki kategorii
            var completedCourses = _specialization.RequiredCourses.Count(c => c.IsCompleted);
            CoursesLabel = $"{completedCourses}/{_specialization.RequiredCourses.Count} ukończonych";

            var completedInternships = _specialization.RequiredInternships.Count(i => i.IsCompleted);
            InternshipsLabel = $"{completedInternships}/{_specialization.RequiredInternships.Count} ukończonych";

            var proceduresTypeA = _specialization.RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeA).ToList();
            var totalProceduresTypeA = proceduresTypeA.Sum(p => p.RequiredCount);
            var completedProceduresTypeA = proceduresTypeA.Sum(p => p.CompletedCount);
            ProceduresALabel = $"{completedProceduresTypeA}/{totalProceduresTypeA} wykonanych";

            var proceduresTypeB = _specialization.RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeB).ToList();
            var totalProceduresTypeB = proceduresTypeB.Sum(p => p.RequiredCount);
            var completedProceduresTypeB = proceduresTypeB.Sum(p => p.CompletedCount);
            ProceduresBLabel = $"{completedProceduresTypeB}/{totalProceduresTypeB} wykonanych";

            // Get statistics from services
            var totalDutyHours = await _dutyShiftService.GetTotalDutyHoursAsync();
            var requiredDutyHours = _specialization.RequiredDutyHoursPerWeek * (_specialization.BaseDurationWeeks / 52.0) * 52;
            DutyShiftsLabel = $"{totalDutyHours:F1}/{requiredDutyHours:F0} godzin";

            var totalSelfEducationDays = await _selfEducationService.GetTotalUsedDaysAsync();
            var totalAllowedDays = _specialization.SelfEducationDaysPerYear * 3; // 3 years typical
            SelfEducationLabel = $"{totalSelfEducationDays}/{totalAllowedDays} dni";

            // Show upcoming events
            await UpdateUpcomingEventsAsync();
        }

        private double GetBasicModuleProgress()
        {
            var basicCourses = _specialization.RequiredCourses.Where(c => c.Module == ModuleType.Basic);
            var basicInternships = _specialization.RequiredInternships.Where(i => i.Module == ModuleType.Basic);
            var basicProcedures = _specialization.RequiredProcedures.Where(p => p.Module == ModuleType.Basic);

            var completedCourses = basicCourses.Count(c => c.IsCompleted);
            var totalCourses = basicCourses.Count();
            var coursesProgress = totalCourses > 0 ? (double)completedCourses / totalCourses : 0;

            var completedInternships = basicInternships.Count(i => i.IsCompleted);
            var totalInternships = basicInternships.Count();
            var internshipsProgress = totalInternships > 0 ? (double)completedInternships / totalInternships : 0;

            var completedProceduresA = basicProcedures
                .Where(p => p.ProcedureType == ProcedureType.TypeA)
                .Sum(p => p.CompletedCount);
            var totalProceduresA = basicProcedures
                .Where(p => p.ProcedureType == ProcedureType.TypeA)
                .Sum(p => p.RequiredCount);
            var proceduresAProgress = totalProceduresA > 0 ? (double)completedProceduresA / totalProceduresA : 0;

            var completedProceduresB = basicProcedures
                .Where(p => p.ProcedureType == ProcedureType.TypeB)
                .Sum(p => p.CompletedCount);
            var totalProceduresB = basicProcedures
                .Where(p => p.ProcedureType == ProcedureType.TypeB)
                .Sum(p => p.RequiredCount);
            var proceduresBProgress = totalProceduresB > 0 ? (double)completedProceduresB / totalProceduresB : 0;

            // Average progress across all categories
            return (coursesProgress + internshipsProgress + proceduresAProgress + proceduresBProgress) / 4;
        }

        private double GetSpecialisticModuleProgress()
        {
            var specCourses = _specialization.RequiredCourses.Where(c => c.Module == ModuleType.Specialistic);
            var specInternships = _specialization.RequiredInternships.Where(i => i.Module == ModuleType.Specialistic);
            var specProcedures = _specialization.RequiredProcedures.Where(p => p.Module == ModuleType.Specialistic);

            var completedCourses = specCourses.Count(c => c.IsCompleted);
            var totalCourses = specCourses.Count();
            var coursesProgress = totalCourses > 0 ? (double)completedCourses / totalCourses : 0;

            var completedInternships = specInternships.Count(i => i.IsCompleted);
            var totalInternships = specInternships.Count();
            var internshipsProgress = totalInternships > 0 ? (double)completedInternships / totalInternships : 0;

            var completedProceduresA = specProcedures
                .Where(p => p.ProcedureType == ProcedureType.TypeA)
                .Sum(p => p.CompletedCount);
            var totalProceduresA = specProcedures
                .Where(p => p.ProcedureType == ProcedureType.TypeA)
                .Sum(p => p.RequiredCount);
            var proceduresAProgress = totalProceduresA > 0 ? (double)completedProceduresA / totalProceduresA : 0;

            var completedProceduresB = specProcedures
                .Where(p => p.ProcedureType == ProcedureType.TypeB)
                .Sum(p => p.CompletedCount);
            var totalProceduresB = specProcedures
                .Where(p => p.ProcedureType == ProcedureType.TypeB)
                .Sum(p => p.RequiredCount);
            var proceduresBProgress = totalProceduresB > 0 ? (double)completedProceduresB / totalProceduresB : 0;

            // Average progress across all categories
            return (coursesProgress + internshipsProgress + proceduresAProgress + proceduresBProgress) / 4;
        }

        private async Task UpdateUpcomingEventsAsync()
        {
            var today = DateTime.Now.Date;
            var upcomingEvents = new List<(DateTime Date, string Description, bool IsImportant)>();

            // Add courses
            foreach (var course in _specialization.RequiredCourses.Where(c => !c.IsCompleted && c.ScheduledDate.HasValue))
            {
                if (course.ScheduledDate.Value >= today)
                {
                    upcomingEvents.Add((
                        course.ScheduledDate.Value,
                        $"Kurs: {course.Name}",
                        false
                    ));
                }
            }

            // Add internships
            foreach (var internship in _specialization.RequiredInternships.Where(i => !i.IsCompleted && i.StartDate.HasValue))
            {
                if (internship.StartDate.Value >= today)
                {
                    upcomingEvents.Add((
                        internship.StartDate.Value,
                        $"Staż: {internship.Name}",
                        false
                    ));
                }
            }

            // Add end of basic module (if in future)
            var endOfBasicModule = _specialization.StartDate.AddDays(_specialization.BasicModuleDurationWeeks * 7);
            if (endOfBasicModule >= today)
            {
                upcomingEvents.Add((
                    endOfBasicModule,
                    "Koniec modułu podstawowego",
                    true
                ));
            }

            // Sort events by date
            upcomingEvents = upcomingEvents.OrderBy(e => e.Date).ToList();

            // Display up to 3 upcoming events
            if (upcomingEvents.Count > 0 && upcomingEvents.Count >= 1)
            {
                UpcomingEvent1 = $"{upcomingEvents[0].Date.ToString("dd.MM.yyyy")} - {upcomingEvents[0].Description}";
                UpcomingEvent1Color = upcomingEvents[0].IsImportant ? Colors.Red : new Color(8, 32, 68);
            }
            else
            {
                UpcomingEvent1 = "Brak nadchodzących wydarzeń";
                UpcomingEvent1Color = new Color(8, 32, 68);
            }

            if (upcomingEvents.Count >= 2)
            {
                UpcomingEvent2 = $"{upcomingEvents[1].Date.ToString("dd.MM.yyyy")} - {upcomingEvents[1].Description}";
                UpcomingEvent2Color = upcomingEvents[1].IsImportant ? Colors.Red : new Color(8, 32, 68);
                UpcomingEvent2Visible = true;
            }
            else
            {
                UpcomingEvent2Visible = false;
            }

            if (upcomingEvents.Count >= 3)
            {
                UpcomingEvent3 = $"{upcomingEvents[2].Date.ToString("dd.MM.yyyy")} - {upcomingEvents[2].Description}";
                UpcomingEvent3Color = upcomingEvents[2].IsImportant ? Colors.Red : new Color(8, 32, 68);
                UpcomingEvent3Visible = true;
            }
            else
            {
                UpcomingEvent3Visible = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToCoursesAsync()
        {
            await Shell.Current.GoToAsync("//CoursesPage");
        }

        [RelayCommand]
        private async Task NavigateToInternshipsAsync()
        {
            await Shell.Current.GoToAsync("//InternshipsPage");
        }

        [RelayCommand]
        private async Task NavigateToProceduresAsync()
        {
            await Shell.Current.GoToAsync("//ProceduresPage");
        }

        [RelayCommand]
        private async Task NavigateToDutyShiftsAsync()
        {
            await Shell.Current.GoToAsync("//DutyShiftsPage");
        }

        [RelayCommand]
        private async Task NavigateToSelfEducationAsync()
        {
            await Shell.Current.GoToAsync("//SelfEducationPage");
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            await Shell.Current.GoToAsync("//SMKExportPage");
        }

        [RelayCommand]
        private async Task NavigateToSettingsAsync()
        {
            await Shell.Current.GoToAsync("//SettingsPage");
        }

        [RelayCommand]
        private async Task ManageAbsencesAsync()
        {
            await Shell.Current.GoToAsync("//AbsenceManagementPage");
        }
    }
}