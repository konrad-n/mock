using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Monitoring;
using System.Collections.ObjectModel;
using System.Text;

namespace SledzSpecke.App.ViewModels.Reports
{
    public partial class ReportsViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IDutyService _dutyService;
        private readonly IProcedureService _procedureService;
        private readonly ISpecializationService _specializationService;
        private readonly ISpecializationRequirementsProvider _requirementsProvider;

        public ReportsViewModel(
            IUserService userService,
            IDutyService dutyService,
            IProcedureService procedureService,
            ISpecializationService specializationService,
            ISpecializationRequirementsProvider requirementsProvider)
        {
            _userService = userService;
            _dutyService = dutyService;
            _procedureService = procedureService;
            _specializationService = specializationService;
            _requirementsProvider = requirementsProvider;

            Title = "Raporty";
            ReportTypes = new ObservableCollection<string>
            {
                "Postęp procedur",
                "Statystyki dyżurów",
                "Braki w wymaganiach specjalizacji",
                "Pełny raport postępu specjalizacji"
            };

            StartDate = DateTime.Today.AddMonths(-3);
            EndDate = DateTime.Today;
        }

        [ObservableProperty]
        private ObservableCollection<string> reportTypes;

        [ObservableProperty]
        private string selectedReportType;

        [ObservableProperty]
        private DateTime startDate;

        [ObservableProperty]
        private DateTime endDate;

        [ObservableProperty]
        private bool showDateFilter;

        [ObservableProperty]
        private bool reportGenerated;

        [ObservableProperty]
        private string reportTitle;

        [ObservableProperty]
        private string reportContent;

        partial void OnSelectedReportTypeChanged(string value)
        {
            ShowDateFilter = value == "Statystyki dyżurów";
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            if (IsBusy || string.IsNullOrEmpty(SelectedReportType))
                return;

            try
            {
                IsBusy = true;

                switch (SelectedReportType)
                {
                    case "Postęp procedur":
                        await GenerateProcedureReportAsync();
                        break;
                    case "Statystyki dyżurów":
                        await GenerateDutyReportAsync();
                        break;
                    case "Braki w wymaganiach specjalizacji":
                        await GenerateDeficienciesReportAsync();
                        break;
                    case "Pełny raport postępu specjalizacji":
                        await GenerateFullProgressReportAsync();
                        break;
                }

                ReportGenerated = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się wygenerować raportu: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GenerateProcedureReportAsync()
        {
            ReportTitle = "Raport postępu procedur";

            var user = await _userService.GetCurrentUserAsync();
            if (user?.CurrentSpecializationId == null)
            {
                ReportContent = "Brak danych o specjalizacji.";
                return;
            }

            // Pobierz wymagane procedury z "source of truth"
            var requiredProcedures = _requirementsProvider.GetRequiredProceduresBySpecialization(user.CurrentSpecializationId.Value);

            // Pobierz wykonane procedury
            var procedures = await _procedureService.GetUserProceduresAsync();

            // Przekształć na format wymagany przez ProcedureMonitoring
            var completedProcedures = new Dictionary<string, ProcedureMonitoring.ProcedureProgress>();

            foreach (var procedure in procedures)
            {
                if (!completedProcedures.ContainsKey(procedure.Name))
                {
                    completedProcedures[procedure.Name] = new ProcedureMonitoring.ProcedureProgress
                    {
                        ProcedureName = procedure.Name,
                        CompletedCount = 0,
                        AssistanceCount = 0,
                        SimulationCount = 0,
                        Executions = new List<ProcedureMonitoring.ProcedureExecution>()
                    };
                }

                var progress = completedProcedures[procedure.Name];

                // Dodaj informacje o wykonaniu
                var execution = new ProcedureMonitoring.ProcedureExecution
                {
                    ExecutionDate = procedure.ExecutionDate,
                    Type = procedure.Type.ToString(),
                    Location = procedure.Location,
                    Notes = procedure.Notes
                };

                if (!string.IsNullOrEmpty(procedure.Notes) && procedure.Notes.Contains("Opiekun:"))
                {
                    var supervisorLine = procedure.Notes.Split('\n')
                        .FirstOrDefault(l => l.StartsWith("Opiekun:"));

                    if (supervisorLine != null)
                    {
                        execution.SupervisorName = supervisorLine.Substring("Opiekun:".Length).Trim();
                    }
                }

                progress.Executions.Add(execution);

                // Aktualizuj liczniki
                if (procedure.Type == Core.Models.Enums.ProcedureType.Execution)
                {
                    if (procedure.IsSimulation)
                    {
                        progress.SimulationCount++;
                    }
                    else
                    {
                        progress.CompletedCount++;
                    }
                }
                else
                {
                    progress.AssistanceCount++;
                }
            }

            // Użyj klasy monitorującej do wygenerowania raportu
            var verifier = new ProcedureMonitoring.ProgressVerification(requiredProcedures);
            var summary = verifier.GenerateProgressSummary(completedProcedures);

            // Wykorzystaj wbudowaną metodę do generowania raportu
            ReportContent = summary.GenerateReport();
        }

        private async Task GenerateDutyReportAsync()
        {
            ReportTitle = "Raport statystyk dyżurów";

            var user = await _userService.GetCurrentUserAsync();
            if (user?.CurrentSpecializationId == null)
            {
                ReportContent = "Brak danych o specjalizacji.";
                return;
            }

            // Pobierz dyżury z zakresu dat
            var duties = await _dutyService.GetUserDutiesAsync(StartDate);
            duties = duties.Where(d => d.StartTime >= StartDate && d.StartTime <= EndDate).ToList();

            // Przekształć na format wymagany przez DutyMonitoring
            var monitoringDuties = duties.Select(d => new DutyMonitoring.Duty
            {
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                Type = d.Type.ToString(),
                Location = d.Location,
                WasSupervised = d.SupervisorId.HasValue
            }).ToList();

            // Użyj klasy monitorującej do walidacji i generowania statystyk
            var validator = new DutyMonitoring.DutyValidator();
            var statistics = validator.GenerateStatistics(monitoringDuties);

            // Wygeneruj raport
            ReportContent = validator.GenerateReport(statistics);

            // Zweryfikuj zgodność z wymaganiami
            var requirements = _requirementsProvider.GetDutyRequirementsBySpecialization(user.CurrentSpecializationId.Value);
            var yearRequirement = requirements.FirstOrDefault(r => r.Year == 1);  // Zakładamy rok 1 dla uproszczenia

            if (yearRequirement != null)
            {
                var (isCompliant, deficiencies) = validator.CheckMonthlyCompliance(
                    user.CurrentSpecializationId.Value,
                    1, // Year 1
                    monitoringDuties);

                // Dodaj informacje o zgodności z wymaganiami
                ReportContent += "\n\nZgodność z wymaganiami: " + (isCompliant ? "Tak" : "Nie");

                if (!isCompliant && deficiencies.Any())
                {
                    ReportContent += "\n\nBraki:";
                    foreach (var deficiency in deficiencies)
                    {
                        ReportContent += $"\n- {deficiency}";
                    }
                }
            }
        }

        private async Task GenerateDeficienciesReportAsync()
        {
            ReportTitle = "Raport braków w wymaganiach specjalizacji";

            var user = await _userService.GetCurrentUserAsync();
            if (user?.CurrentSpecializationId == null)
            {
                ReportContent = "Brak danych o specjalizacji.";
                return;
            }

            var sb = new StringBuilder();

            // 1. Sprawdź braki w procedurach
            var requiredProcedures = _requirementsProvider.GetRequiredProceduresBySpecialization(user.CurrentSpecializationId.Value);
            var procedures = await _procedureService.GetUserProceduresAsync();

            // Przygotuj dane dla weryfikacji
            var completedProcedures = new Dictionary<string, ProcedureMonitoring.ProcedureProgress>();
            foreach (var procedure in procedures)
            {
                if (!completedProcedures.ContainsKey(procedure.Name))
                {
                    completedProcedures[procedure.Name] = new ProcedureMonitoring.ProcedureProgress
                    {
                        ProcedureName = procedure.Name,
                        CompletedCount = 0,
                        AssistanceCount = 0,
                        SimulationCount = 0
                    };
                }

                var progressCompleted = completedProcedures[procedure.Name];

                if (procedure.Type == Core.Models.Enums.ProcedureType.Execution)
                {
                    if (procedure.IsSimulation)
                    {
                        progressCompleted.SimulationCount++;
                    }
                    else
                    {
                        progressCompleted.CompletedCount++;
                    }
                }
                else
                {
                    progressCompleted.AssistanceCount++;
                }
            }

            // Weryfikuj braki
            var verifier = new ProcedureMonitoring.ProgressVerification(requiredProcedures);
            var (isComplete, procedureDeficiencies) = verifier.VerifyProgress(completedProcedures);

            sb.AppendLine("=== BRAKI W PROGRAMIE SPECJALIZACJI ===\n");
            sb.AppendLine("PROCEDURY:");

            if (procedureDeficiencies.Any())
            {
                foreach (var deficiency in procedureDeficiencies)
                {
                    sb.AppendLine($"- {deficiency}");
                }
            }
            else
            {
                sb.AppendLine("Brak braków w zakresie wykonanych procedur.");
            }

            // 2. Sprawdź braki w dyżurach
            sb.AppendLine("\nDYŻURY:");

            var duties = await _dutyService.GetUserDutiesAsync();
            var monitoringDuties = duties.Select(d => new DutyMonitoring.Duty
            {
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                Type = d.Type.ToString(),
                Location = d.Location,
                WasSupervised = d.SupervisorId.HasValue
            }).ToList();

            var dutyValidator = new DutyMonitoring.DutyValidator();
            var dutyStats = dutyValidator.GenerateStatistics(monitoringDuties);

            var requirements = _requirementsProvider.GetDutyRequirementsBySpecialization(user.CurrentSpecializationId.Value);
            bool anyDutyDeficiencies = false;

            foreach (var yearRequirement in requirements)
            {
                var (isYearCompliant, yearDeficiencies) = dutyValidator.CheckMonthlyCompliance(
                    user.CurrentSpecializationId.Value,
                    yearRequirement.Year,
                    monitoringDuties);

                if (!isYearCompliant && yearDeficiencies.Any())
                {
                    anyDutyDeficiencies = true;
                    sb.AppendLine($"\nRok {yearRequirement.Year}:");
                    foreach (var deficiency in yearDeficiencies)
                    {
                        sb.AppendLine($"- {deficiency}");
                    }
                }
            }

            if (!anyDutyDeficiencies)
            {
                sb.AppendLine("Brak braków w zakresie dyżurów.");
            }

            // 3. Sprawdź braki w kursach i stażach
            sb.AppendLine("\nKURSY I STAŻE:");

            var progress = await _specializationService.GetProgressStatisticsAsync(user.CurrentSpecializationId.Value);

            if (progress.CoursesProgress < 1.0)
            {
                sb.AppendLine($"- Ukończono tylko {progress.CoursesProgress:P0} wymaganych kursów");
            }

            if (progress.InternshipsProgress < 1.0)
            {
                sb.AppendLine($"- Ukończono tylko {progress.InternshipsProgress:P0} wymaganych staży");
            }

            if (progress.CoursesProgress >= 1.0 && progress.InternshipsProgress >= 1.0)
            {
                sb.AppendLine("Brak braków w zakresie kursów i staży.");
            }

            ReportContent = sb.ToString();
        }

        private async Task GenerateFullProgressReportAsync()
        {
            ReportTitle = "Pełny raport postępu specjalizacji";

            var user = await _userService.GetCurrentUserAsync();
            if (user?.CurrentSpecializationId == null)
            {
                ReportContent = "Brak danych o specjalizacji.";
                return;
            }

            var specialization = await _specializationService.GetSpecializationAsync(user.CurrentSpecializationId.Value);
            var progress = await _specializationService.GetProgressStatisticsAsync(user.CurrentSpecializationId.Value);

            var sb = new StringBuilder();

            sb.AppendLine($"RAPORT POSTĘPU SPECJALIZACJI: {specialization.Name}");
            sb.AppendLine($"Wygenerowano: {DateTime.Now:g}");
            sb.AppendLine($"Lekarz: {user.Name}, PWZ: {user.PWZ}");
            sb.AppendLine($"Data rozpoczęcia: {user.SpecializationStartDate:d}");
            sb.AppendLine($"Przewidywana data zakończenia: {user.ExpectedEndDate:d}");
            sb.AppendLine();

            sb.AppendLine("=== PODSUMOWANIE POSTĘPU ===");
            sb.AppendLine($"Całkowity postęp: {progress.OverallProgress:P0}");
            sb.AppendLine($"Postęp procedur: {progress.ProceduresProgress:P0}");
            sb.AppendLine($"Postęp dyżurów: {progress.DutiesProgress:P0}");
            sb.AppendLine($"Postęp kursów: {progress.CoursesProgress:P0}");
            sb.AppendLine($"Postęp staży: {progress.InternshipsProgress:P0}");

            // Dołącz szczegółowe raporty
            sb.AppendLine("\n=== SZCZEGÓŁY PROCEDUR ===");
            await GenerateProcedureReportAsync();
            sb.AppendLine(ReportContent);

            sb.AppendLine("\n=== SZCZEGÓŁY DYŻURÓW ===");
            StartDate = DateTime.Today.AddYears(-1);
            EndDate = DateTime.Today;
            await GenerateDutyReportAsync();
            sb.AppendLine(ReportContent);

            sb.AppendLine("\n=== BRAKI I ZALECENIA ===");
            await GenerateDeficienciesReportAsync();
            sb.AppendLine(ReportContent);

            ReportContent = sb.ToString();
        }

        [RelayCommand]
        private async Task ExportToPdfAsync()
        {
            await Shell.Current.DisplayAlert("Eksport do PDF",
                "Funkcja eksportu do PDF zostanie zaimplementowana wkrótce.", "OK");
        }

        [RelayCommand]
        private async Task ShareReportAsync()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = ReportContent,
                Title = ReportTitle
            });
        }
    }
}
