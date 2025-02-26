using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Monitoring;
using System.Collections.ObjectModel;
using static SledzSpecke.Core.Models.Monitoring.ProcedureMonitoring;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public partial class ProceduresViewModel : BaseViewModel
    {
        private readonly IProcedureService _procedureService;
        private readonly ISpecializationService _specializationService;
        private readonly ISpecializationRequirementsProvider _requirementsProvider;

        public ProceduresViewModel(
            IProcedureService procedureService,
            ISpecializationService specializationService,
            ISpecializationRequirementsProvider requirementsProvider)
        {
            _procedureService = procedureService;
            _specializationService = specializationService;
            _requirementsProvider = requirementsProvider;

            Title = "Procedury";
            Procedures = new ObservableCollection<Core.Models.Domain.ProcedureExecution>();
            Categories = new ObservableCollection<string>();
            Stages = new ObservableCollection<string>();
            CategoriesBySpecialization = new ObservableCollection<string>();
            StagesBySpecialization = new ObservableCollection<string>();
        }

        [ObservableProperty]
        private ObservableCollection<Core.Models.Domain.ProcedureExecution> procedures;

        [ObservableProperty]
        private ObservableCollection<string> categories;

        [ObservableProperty]
        private ObservableCollection<string> stages;

        [ObservableProperty]
        private ObservableCollection<string> categoriesBySpecialization;

        [ObservableProperty]
        private ObservableCollection<string> stagesBySpecialization;

        [ObservableProperty]
        private string selectedCategory;

        [ObservableProperty]
        private string selectedStage;

        [ObservableProperty]
        private double completionPercentage;

        [ObservableProperty]
        private Dictionary<string, (int Required, int Completed, int Assisted)> categoryProgress;

        [ObservableProperty]
        private Dictionary<string, (int Required, int Completed, int Assisted)> stageProgress;

        [ObservableProperty]
        private ProcedureMonitoring.ProgressSummary progressSummary;

        public string OverallCompletionDisplay
        {
            get
            {
                if (ProgressSummary == null)
                    return "0% ukończone";

                return $"{ProgressSummary.OverallCompletionPercentage:P0} ukończone";
            }
        }

        public string ProceduresCompletedDisplay
        {
            get
            {
                if (ProgressSummary == null)
                    return "Wykonane procedury: 0/0";

                return $"Wykonane procedury: {ProgressSummary.TotalProceduresCompleted}/{ProgressSummary.TotalProceduresRequired}";
            }
        }

        private int _currentSpecializationId;

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Pobierz bieżącą specjalizację
                var currentSpecialization = await _specializationService.GetCurrentSpecializationAsync();
                if (currentSpecialization != null)
                {
                    _currentSpecializationId = currentSpecialization.Id;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Brak specjalizacji", "Nie wybrano żadnej specjalizacji.", "OK");
                    return;
                }

                // Załaduj procedury
                var userProcedures = await _procedureService.GetUserProceduresAsync();
                Procedures.Clear();
                foreach (var procedure in userProcedures)
                {
                    Procedures.Add(procedure);
                }

                // Załaduj kategorie i etapy specyficzne dla specjalizacji
                await LoadSpecializationFiltersAsync();

                // Załaduj postępy
                CompletionPercentage = await _procedureService.GetProcedureCompletionPercentageAsync();
                CategoryProgress = await _procedureService.GetProcedureProgressByCategoryAsync();
                StageProgress = await _procedureService.GetProcedureProgressByStageAsync();

                // Oblicz postęp procedur z wymaganiami specjalizacji
                await CalculateProgressSummaryAsync();

                // Ustaw domyślne filtrowanie
                SelectedCategory = "Wszystkie";
                SelectedStage = "Wszystkie";

                // Powiadomienie, że właściwości do wyświetlania (OverallCompletionDisplay itp.) mogą się zmienić
                OnPropertyChanged(nameof(OverallCompletionDisplay));
                OnPropertyChanged(nameof(ProceduresCompletedDisplay));
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadSpecializationFiltersAsync()
        {
            try
            {
                // Pobierz wymagania procedur dla tej specjalizacji
                var procedureRequirements = _requirementsProvider.GetRequiredProceduresBySpecialization(_currentSpecializationId);

                // Dodaj dostępne kategorie i etapy
                CategoriesBySpecialization.Clear();
                StagesBySpecialization.Clear();

                // Dodaj opcję "Wszystkie" na początku
                CategoriesBySpecialization.Add("Wszystkie");
                StagesBySpecialization.Add("Wszystkie");

                // Zbierz unikalne kategorie i etapy
                var uniqueCategories = new HashSet<string>();
                var uniqueStages = new HashSet<string>();

                foreach (var category in procedureRequirements.Keys)
                {
                    if (!string.IsNullOrEmpty(category))
                        uniqueCategories.Add(category);

                    foreach (var procedure in procedureRequirements[category])
                    {
                        if (procedure.Description != null && procedure.Description.Contains("Etap:"))
                        {
                            var stageParts = procedure.Description.Split("Etap:");
                            if (stageParts.Length > 1)
                            {
                                var stage = stageParts[1].Trim();
                                if (!string.IsNullOrEmpty(stage))
                                    uniqueStages.Add(stage);
                            }
                        }
                    }
                }

                // Dodaj do kolekcji
                foreach (var category in uniqueCategories.OrderBy(c => c))
                {
                    CategoriesBySpecialization.Add(category);
                }

                foreach (var stage in uniqueStages.OrderBy(s => s))
                {
                    StagesBySpecialization.Add(stage);
                }

                // Załaduj także ogólne kategorie i etapy (z serwisu procedur)
                var availableCategories = await _procedureService.GetAvailableCategoriesAsync();
                Categories.Clear();
                Categories.Add("Wszystkie");
                foreach (var category in availableCategories.Where(c => !string.IsNullOrEmpty(c)))
                {
                    Categories.Add(category);
                }

                var availableStages = await _procedureService.GetAvailableStagesAsync();
                Stages.Clear();
                Stages.Add("Wszystkie");
                foreach (var stage in availableStages.Where(s => !string.IsNullOrEmpty(s)))
                {
                    Stages.Add(stage);
                }
            }
            catch (Exception)
            {
                // Ukryj błędy związane z filtrowaniem - to tylko dodatkowa funkcjonalność
            }
        }

        private async Task CalculateProgressSummaryAsync()
        {
            try
            {
                // Pobierz wymagania procedur dla bieżącej specjalizacji
                var procedureRequirements = _requirementsProvider.GetRequiredProceduresBySpecialization(_currentSpecializationId);

                // Przekształć dane o wykonanych procedurach
                var completedProcedures = new Dictionary<string, ProcedureProgress>();

                foreach (var procedure in Procedures)
                {
                    if (!completedProcedures.ContainsKey(procedure.Name))
                    {
                        completedProcedures[procedure.Name] = new ProcedureProgress
                        {
                            ProcedureName = procedure.Name,
                            CompletedCount = 0,
                            AssistanceCount = 0,
                            SimulationCount = 0,
                            Executions = new List<ProcedureExecution>()
                        };
                    }

                    var progress = completedProcedures[procedure.Name];
                    var execution = new ProcedureExecution
                    {
                        ExecutionDate = procedure.ExecutionDate,
                        Type = procedure.Type.ToString(),
                        Location = procedure.Location,
                        Notes = procedure.Notes
                    };

                    // Ekstrakcja informacji o opiekunie z notatek (jeśli jest)
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
                    switch (procedure.Type)
                    {
                        case Core.Models.Enums.ProcedureType.Execution:
                            if (procedure.IsSimulation)
                                progress.SimulationCount++;
                            else
                                progress.CompletedCount++;
                            break;
                        case Core.Models.Enums.ProcedureType.Assistance:
                            progress.AssistanceCount++;
                            break;
                    }
                }

                // Utwórz instancję weryfikatora
                var verifier = new ProgressVerification(procedureRequirements);

                // Generuj podsumowanie
                ProgressSummary = verifier.GenerateProgressSummary(completedProcedures);

                // Po aktualizacji ProgressSummary również zasygnalizuj zmianę
                OnPropertyChanged(nameof(OverallCompletionDisplay));
                OnPropertyChanged(nameof(ProceduresCompletedDisplay));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas obliczania podsumowania postępu: {ex.Message}");
                // Nie pokazuj błędu użytkownikowi - to tylko statystyki
            }
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            _ = FilterProceduresAsync();
        }

        partial void OnSelectedStageChanged(string value)
        {
            _ = FilterProceduresAsync();
        }

        private async Task FilterProceduresAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var allProcedures = await _procedureService.GetUserProceduresAsync();

                // Filtruj według kategorii i etapu
                var filteredProcedures = allProcedures;
                if (SelectedCategory != "Wszystkie" && SelectedCategory != null)
                {
                    filteredProcedures = filteredProcedures
                        .Where(p => p.Category == SelectedCategory)
                        .ToList();
                }

                if (SelectedStage != "Wszystkie" && SelectedStage != null)
                {
                    filteredProcedures = filteredProcedures
                        .Where(p => p.Stage == SelectedStage)
                        .ToList();
                }

                Procedures.Clear();
                foreach (var procedure in filteredProcedures)
                {
                    Procedures.Add(procedure);
                }
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zafiltrować danych: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddProcedureAsync()
        {
            await Shell.Current.GoToAsync("procedure/add");
        }

        [RelayCommand]
        private async Task ViewProcedureAsync(int id)
        {
            await Shell.Current.GoToAsync($"procedure/edit?id={id}");
        }

        [RelayCommand]
        private async Task ExportProceduresAsync()
        {
            if (ProgressSummary != null)
            {
                var report = ProgressSummary.GenerateReport();
                await Shell.Current.DisplayAlert("Raport postępu procedur", report, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Eksport", "Funkcja eksportu zostanie zaimplementowana wkrótce.", "OK");
            }
        }
    }
}
