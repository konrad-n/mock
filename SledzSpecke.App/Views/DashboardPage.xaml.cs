using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Views
{
    public partial class DashboardPage : ContentPage
    {
        private Specialization _specialization;

        public DashboardPage()
        {
            InitializeComponent();
            LoadSpecializationData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadSpecializationData();
        }

        private async void LoadSpecializationData()
        {
            try
            {
                _specialization = await App.SpecializationService.GetSpecializationAsync();
                LoadDashboardData();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load specialization data: {ex.Message}", "OK");
            }
        }

        private async void LoadDashboardData()
        {
            // Podstawowe informacje o specjalizacji
            StartDateLabel.Text = _specialization.StartDate.ToString("dd-MM-yyyy");
            EndDateLabel.Text = _specialization.ExpectedEndDate.ToString("dd-MM-yyyy");

            var daysLeft = (_specialization.ExpectedEndDate - DateTime.Now).Days;
            DaysLeftLabel.Text = daysLeft.ToString();

            // Określenie obecnego etapu
            var daysSinceStart = (DateTime.Now - _specialization.StartDate).Days;
            if (daysSinceStart < (_specialization.BasicModuleDurationWeeks * 7))
            {
                CurrentStageLabel.Text = "Moduł podstawowy";
            }
            else
            {
                CurrentStageLabel.Text = "Moduł specjalistyczny";
            }

            // Ogólny postęp
            var totalProgress = _specialization.GetCompletionPercentage() / 100;
            TotalProgressBar.Progress = totalProgress;
            TotalProgressLabel.Text = $"{(totalProgress * 100):F0}% ukończono";

            // Postęp modułów
            var basicModuleProgress = GetBasicModuleProgress();
            BasicModuleProgressBar.Progress = basicModuleProgress;
            BasicModuleProgressLabel.Text = $"{(basicModuleProgress * 100):F0}%";

            var specialisticModuleProgress = GetSpecialisticModuleProgress();
            SpecialisticModuleProgressBar.Progress = specialisticModuleProgress;
            SpecialisticModuleProgressLabel.Text = $"{(specialisticModuleProgress * 100):F0}%";

            // Statystyki kategorii
            var completedCourses = _specialization.RequiredCourses.Count(c => c.IsCompleted);
            CoursesLabel.Text = $"{completedCourses}/{_specialization.RequiredCourses.Count} ukończonych";

            var completedInternships = _specialization.RequiredInternships.Count(i => i.IsCompleted);
            InternshipsLabel.Text = $"{completedInternships}/{_specialization.RequiredInternships.Count} ukończonych";

            var proceduresTypeA = _specialization.RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeA).ToList();
            var totalProceduresTypeA = proceduresTypeA.Sum(p => p.RequiredCount);
            var completedProceduresTypeA = proceduresTypeA.Sum(p => p.CompletedCount);
            ProceduresALabel.Text = $"{completedProceduresTypeA}/{totalProceduresTypeA} wykonanych";

            var proceduresTypeB = _specialization.RequiredProcedures.Where(p => p.ProcedureType == ProcedureType.TypeB).ToList();
            var totalProceduresTypeB = proceduresTypeB.Sum(p => p.RequiredCount);
            var completedProceduresTypeB = proceduresTypeB.Sum(p => p.CompletedCount);
            ProceduresBLabel.Text = $"{completedProceduresTypeB}/{totalProceduresTypeB} wykonanych";

            // Get statistics from services
            var totalDutyHours = await App.DutyShiftService.GetTotalDutyHoursAsync();
            var requiredDutyHours = _specialization.RequiredDutyHoursPerWeek * (_specialization.BaseDurationWeeks / 52.0) * 52;
            DutyShiftsLabel.Text = $"{totalDutyHours:F1}/{requiredDutyHours:F0} godzin";

            var totalSelfEducationDays = await App.SelfEducationService.GetTotalUsedDaysAsync();
            var totalAllowedDays = _specialization.SelfEducationDaysPerYear * 3; // 3 years typical
            SelfEducationLabel.Text = $"{totalSelfEducationDays}/{totalAllowedDays} dni";

            // Show upcoming events
            UpdateUpcomingEvents();
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

        private void UpdateUpcomingEvents()
        {
            var today = DateTime.Now.Date;
            var upcomingEvents = new List<Tuple<DateTime, string, bool>>(); // date, description, is important

            // Add courses
            foreach (var course in _specialization.RequiredCourses.Where(c => !c.IsCompleted && c.ScheduledDate.HasValue))
            {
                if (course.ScheduledDate.Value >= today)
                {
                    upcomingEvents.Add(new Tuple<DateTime, string, bool>(
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
                    upcomingEvents.Add(new Tuple<DateTime, string, bool>(
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
                upcomingEvents.Add(new Tuple<DateTime, string, bool>(
                    endOfBasicModule,
                    "Koniec modułu podstawowego",
                    true
                ));
            }

            // Sort events by date
            upcomingEvents = upcomingEvents.OrderBy(e => e.Item1).ToList();

            // Display up to 3 upcoming events
            if (upcomingEvents.Count > 0 && upcomingEvents.Count >= 1)
            {
                UpcomingEvent1.Text = $"{upcomingEvents[0].Item1.ToString("dd.MM.yyyy")} - {upcomingEvents[0].Item2}";
                UpcomingEvent1.TextColor = upcomingEvents[0].Item3 ? Colors.Red : new Color(8,32,68);
            }
            else
            {
                UpcomingEvent1.Text = "Brak nadchodzących wydarzeń";
            }

            if (upcomingEvents.Count >= 2)
            {
                UpcomingEvent2.Text = $"{upcomingEvents[1].Item1.ToString("dd.MM.yyyy")} - {upcomingEvents[1].Item2}";
                UpcomingEvent2.TextColor = upcomingEvents[1].Item3 ? Colors.Red : new Color(8,32,68);
                UpcomingEvent2.IsVisible = true;
            }
            else
            {
                UpcomingEvent2.IsVisible = false;
            }

            if (upcomingEvents.Count >= 3)
            {
                UpcomingEvent3.Text = $"{upcomingEvents[2].Item1.ToString("dd.MM.yyyy")} - {upcomingEvents[2].Item2}";
                UpcomingEvent3.TextColor = upcomingEvents[2].Item3 ? Colors.Red : new Color(8,32,68);
                UpcomingEvent3.IsVisible = true;
            }
            else
            {
                UpcomingEvent3.IsVisible = false;
            }
        }

        private async void OnCoursesButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CoursesPage());
        }

        private async void OnInternshipsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InternshipsPage());
        }

        private async void OnProceduresButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProceduresPage());
        }

        private async void OnDutyShiftsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DutyShiftsPage());
        }

        private async void OnSelfEducationButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SelfEducationPage());
        }

        private async void OnGenerateReportButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SMKExportPage());
        }

        private async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}