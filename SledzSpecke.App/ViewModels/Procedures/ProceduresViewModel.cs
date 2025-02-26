using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public partial class ProceduresViewModel : BaseViewModel
    {
        private readonly IProcedureService _procedureService;
        private readonly ISpecializationService _specializationService;

        public ProceduresViewModel(
            IProcedureService procedureService,
            ISpecializationService specializationService)
        {
            _procedureService = procedureService;
            _specializationService = specializationService;

            Title = "Procedury";
            Procedures = new ObservableCollection<ProcedureExecution>();
            Categories = new ObservableCollection<string>();
            Stages = new ObservableCollection<string>();

            // Nowe kolekcje dla kategorii i etapów specyficznych dla specjalizacji
            CategoriesBySpecialization = new ObservableCollection<string>();
            StagesBySpecialization = new ObservableCollection<string>();
        }

        [ObservableProperty]
        private ObservableCollection<ProcedureExecution> procedures;

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

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Załaduj procedury
                var userProcedures = await _procedureService.GetUserProceduresAsync();
                Procedures.Clear();
                foreach (var procedure in userProcedures)
                {
                    Procedures.Add(procedure);
                }

                // Załaduj dostępne kategorie i etapy
                var availableCategories = await _procedureService.GetAvailableCategoriesAsync();
                Categories.Clear();
                Categories.Add("Wszystkie");
                foreach (var category in availableCategories)
                {
                    Categories.Add(category);
                }

                var availableStages = await _procedureService.GetAvailableStagesAsync();
                Stages.Clear();
                Stages.Add("Wszystkie");
                foreach (var stage in availableStages)
                {
                    Stages.Add(stage);
                }

                // Załaduj postępy
                CompletionPercentage = await _procedureService.GetProcedureCompletionPercentageAsync();
                CategoryProgress = await _procedureService.GetProcedureProgressByCategoryAsync();
                StageProgress = await _procedureService.GetProcedureProgressByStageAsync();

                // Załaduj kategorie i etapy specyficzne dla specjalizacji
                await LoadSpecializationFiltersAsync();

                // Ustaw domyślne filtrowanie
                SelectedCategory = "Wszystkie";
                SelectedStage = "Wszystkie";
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

        public async Task LoadSpecializationFiltersAsync()
        {
            try
            {
                var specialization = await _specializationService.GetCurrentSpecializationAsync();

                if (specialization != null)
                {
                    CategoriesBySpecialization.Clear();
                    StagesBySpecialization.Clear();

                    // Załaduj kategorie specyficzne dla specjalizacji
                    var requirements = await _specializationService.GetRequiredProceduresAsync(specialization.Id);

                    var specialtyCategories = requirements
                        .Select(r => r.Category)
                        .Where(c => !string.IsNullOrEmpty(c))
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList();

                    var specialtyStages = requirements
                        .Select(r => r.Stage)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct()
                        .OrderBy(s => s)
                        .ToList();

                    // Dodaj opcję "Wszystkie" na początku
                    CategoriesBySpecialization.Add("Wszystkie");
                    foreach (var category in specialtyCategories)
                    {
                        CategoriesBySpecialization.Add(category);
                    }

                    StagesBySpecialization.Add("Wszystkie");
                    foreach (var stage in specialtyStages)
                    {
                        StagesBySpecialization.Add(stage);
                    }
                }
            }
            catch (System.Exception)
            {
                // Obsługa błędów - ukryj ten błąd, ponieważ jest to tylko dodatkowa funkcjonalność
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
            // Implementacja eksportu procedur
            await Shell.Current.DisplayAlert("Eksport", "Funkcja eksportu zostanie zaimplementowana wkrótce.", "OK");
        }
    }
}
