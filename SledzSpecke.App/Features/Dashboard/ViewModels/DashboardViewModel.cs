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
        private readonly ISpecializationService specializationService;
        private readonly ISpecializationDateCalculator specializationDateCalculator;
        private readonly IDutyShiftService dutyShiftService;
        private readonly ISelfEducationService selfEducationService;
        private Specialization specialization;

        [ObservableProperty]
        private string startDateLabel;

        [ObservableProperty]
        private string plannedEndDateLabel;

        [ObservableProperty]
        private string actualEndDateLabel;

        [ObservableProperty]
        private string daysLeftLabel;

        [ObservableProperty]
        private string currentStageLabel;

        [ObservableProperty]
        private double totalProgressBarValue;

        [ObservableProperty]
        private string totalProgressLabel;

        [ObservableProperty]
        private double basicModuleProgressBarValue;

        [ObservableProperty]
        private string basicModuleProgressLabel;

        [ObservableProperty]
        private double specialisticModuleProgressBarValue;

        [ObservableProperty]
        private string specialisticModuleProgressLabel;

        [ObservableProperty]
        private string coursesLabel;

        [ObservableProperty]
        private string internshipsLabel;

        [ObservableProperty]
        private string proceduresALabel;

        [ObservableProperty]
        private string proceduresBLabel;

        [ObservableProperty]
        private string dutyShiftsLabel;

        [ObservableProperty]
        private string selfEducationLabel;

        [ObservableProperty]
        private string upcomingEvent1;

        [ObservableProperty]
        private string upcomingEvent2;

        [ObservableProperty]
        private string upcomingEvent3;

        [ObservableProperty]
        private bool upcomingEvent2Visible;

        [ObservableProperty]
        private bool upcomingEvent3Visible;

        [ObservableProperty]
        private Color upcomingEvent1Color;

        [ObservableProperty]
        private Color upcomingEvent2Color;

        [ObservableProperty]
        private Color upcomingEvent3Color;

        public DashboardViewModel(
            ISpecializationService specializationService,
            ISpecializationDateCalculator specializationDateCalculator,
            IDutyShiftService dutyShiftService,
            ISelfEducationService selfEducationService,
            ILogger<DashboardViewModel> logger)
            : base(logger)
        {
            this.specializationService = specializationService;
            this.specializationDateCalculator = specializationDateCalculator;
            this.dutyShiftService = dutyShiftService;
            this.selfEducationService = selfEducationService;

            this.Title = "Dashboard";
        }

        public override async Task InitializeAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.LoadSpecializationDataAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading dashboard data");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadSpecializationDataAsync()
        {
            this.specialization = await this.specializationService.GetSpecializationAsync();

            await this.LoadDashboardDataAsync();
        }

        private async Task LoadDashboardDataAsync()
        {
            this.StartDateLabel = this.specialization.StartDate.ToString("dd-MM-yyyy");
            DateTime plannedEndDate = this.specialization.StartDate.AddDays(this.specialization.BaseDurationWeeks * 7);
            this.PlannedEndDateLabel = plannedEndDate.ToString("dd-MM-yyyy");
            DateTime actualEndDate = plannedEndDate;
            try
            {
                actualEndDate = await this.specializationDateCalculator.CalculateExpectedEndDateAsync(this.specialization.Id);
                this.ActualEndDateLabel = actualEndDate.ToString("dd-MM-yyyy");
            }
            catch (Exception ex)
            {
                this.ActualEndDateLabel = plannedEndDate.ToString("dd-MM-yyyy");
                this.logger.LogError(ex, "Error calculating actual end date");
            }

            var daysLeft = (actualEndDate - DateTime.Now).Days;
            this.DaysLeftLabel = daysLeft > 0 ? daysLeft.ToString() : "0";
            var daysSinceStart = (DateTime.Now - this.specialization.StartDate).Days;
            if (daysSinceStart < (this.specialization.BasicModuleDurationWeeks * 7))
            {
                this.CurrentStageLabel = "Moduł podstawowy";
            }
            else
            {
                this.CurrentStageLabel = "Moduł specjalistyczny";
            }

            var totalProgress = this.specialization.GetCompletionPercentage() / 100;
            this.TotalProgressBarValue = totalProgress;
            this.TotalProgressLabel = $"{totalProgress * 100:F0}% ukończono";
            var basicModuleProgress = this.GetBasicModuleProgress();
            this.BasicModuleProgressBarValue = basicModuleProgress;
            this.BasicModuleProgressLabel = $"{basicModuleProgress * 100:F0}%";
            var specialisticModuleProgress = this.GetSpecialisticModuleProgress();
            this.SpecialisticModuleProgressBarValue = specialisticModuleProgress;
            this.SpecialisticModuleProgressLabel = $"{specialisticModuleProgress * 100:F0}%";
            var completedCourses = this.specialization.RequiredCourses.Count(c => c.IsCompleted);
            this.CoursesLabel = $"{completedCourses}/{this.specialization.RequiredCourses.Count} ukończonych";
            var completedInternships = this.specialization.RequiredInternships.Count(i => i.IsCompleted);
            this.InternshipsLabel = $"{completedInternships}/{this.specialization.RequiredInternships.Count} ukończonych";
            var proceduresTypeA = this.specialization.RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeA).ToList();
            var totalProceduresTypeA = proceduresTypeA.Sum(p => p.RequiredCount);
            var completedProceduresTypeA = proceduresTypeA.Sum(p => p.CompletedCount);
            this.ProceduresALabel = $"{completedProceduresTypeA}/{totalProceduresTypeA} wykonanych";
            var proceduresTypeB = this.specialization.RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeB).ToList();
            var totalProceduresTypeB = proceduresTypeB.Sum(p => p.RequiredCount);
            var completedProceduresTypeB = proceduresTypeB.Sum(p => p.CompletedCount);
            this.ProceduresBLabel = $"{completedProceduresTypeB}/{totalProceduresTypeB} wykonanych";
            var totalDutyHours = await this.dutyShiftService.GetTotalDutyHoursAsync();
            var requiredDutyHours = this.specialization.RequiredDutyHoursPerWeek * (this.specialization.BaseDurationWeeks / 52.0) * 52;
            this.DutyShiftsLabel = $"{totalDutyHours:F1}/{requiredDutyHours:F0} godzin";
            var totalSelfEducationDays = await this.selfEducationService.GetTotalUsedDaysAsync();
            var totalAllowedDays = this.specialization.SelfEducationDaysPerYear * 3;
            this.SelfEducationLabel = $"{totalSelfEducationDays}/{totalAllowedDays} dni";
            await this.UpdateUpcomingEventsAsync();
        }

        private double GetBasicModuleProgress()
        {
            var basicCourses = this.specialization.RequiredCourses.Where(c => c.Module == ModuleType.Basic);
            var basicInternships = this.specialization.RequiredInternships.Where(i => i.Module == ModuleType.Basic);
            var basicProcedures = this.specialization.RequiredProcedures.Where(p => p.Module == ModuleType.Basic);
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
            return (coursesProgress + internshipsProgress + proceduresAProgress + proceduresBProgress) / 4;
        }

        private double GetSpecialisticModuleProgress()
        {
            var specCourses = this.specialization.RequiredCourses.Where(c => c.Module == ModuleType.Specialistic);
            var specInternships = this.specialization.RequiredInternships.Where(i => i.Module == ModuleType.Specialistic);
            var specProcedures = this.specialization.RequiredProcedures.Where(p => p.Module == ModuleType.Specialistic);

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
            return (coursesProgress + internshipsProgress + proceduresAProgress + proceduresBProgress) / 4;
        }

        private async Task UpdateUpcomingEventsAsync()
        {
            var today = DateTime.Now.Date;
            var upcomingEvents = new List<(DateTime Date, string Description, bool IsImportant)>();
            foreach (var course in this.specialization.RequiredCourses.Where(c => !c.IsCompleted && c.ScheduledDate.HasValue))
            {
                if (course.ScheduledDate!.Value >= today)
                {
                    upcomingEvents.Add((
                        course.ScheduledDate.Value,
                        $"Kurs: {course.Name}",
                        false
                    ));
                }
            }

            foreach (var internship in this.specialization.RequiredInternships.Where(i => !i.IsCompleted && i.StartDate.HasValue))
            {
                if (internship.StartDate!.Value >= today)
                {
                    upcomingEvents.Add((
                        internship.StartDate.Value,
                        $"Staż: {internship.Name}",
                        false
                    ));
                }
            }

            var endOfBasicModule = this.specialization.StartDate.AddDays(this.specialization.BasicModuleDurationWeeks * 7);
            if (endOfBasicModule >= today)
            {
                upcomingEvents.Add((
                    endOfBasicModule,
                    "Koniec modułu podstawowego",
                    true
                ));
            }

            upcomingEvents = upcomingEvents.OrderBy(e => e.Date).ToList();

            if (upcomingEvents.Count > 0 && upcomingEvents.Count >= 1)
            {
                this.UpcomingEvent1 = $"{upcomingEvents[0].Date.ToString("dd.MM.yyyy")} - {upcomingEvents[0].Description}";
                this.UpcomingEvent1Color = upcomingEvents[0].IsImportant ? Colors.Red : new Color(8, 32, 68);
            }
            else
            {
                this.UpcomingEvent1 = "Brak nadchodzących wydarzeń";
                this.UpcomingEvent1Color = new Color(8, 32, 68);
            }

            if (upcomingEvents.Count >= 2)
            {
                this.UpcomingEvent2 = $"{upcomingEvents[1].Date.ToString("dd.MM.yyyy")} - {upcomingEvents[1].Description}";
                this.UpcomingEvent2Color = upcomingEvents[1].IsImportant ? Colors.Red : new Color(8, 32, 68);
                this.UpcomingEvent2Visible = true;
            }
            else
            {
                this.UpcomingEvent2Visible = false;
            }

            if (upcomingEvents.Count >= 3)
            {
                this.UpcomingEvent3 = $"{upcomingEvents[2].Date.ToString("dd.MM.yyyy")} - {upcomingEvents[2].Description}";
                this.UpcomingEvent3Color = upcomingEvents[2].IsImportant ? Colors.Red : new Color(8, 32, 68);
                this.UpcomingEvent3Visible = true;
            }
            else
            {
                this.UpcomingEvent3Visible = false;
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