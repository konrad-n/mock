using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public partial class ProcedureAddViewModel : BaseViewModel
    {
        private readonly IProcedureService _procedureService;
        private readonly ISpecializationService _specializationService;
        private readonly ISpecializationRequirementsProvider _requirementsProvider;

        public ProcedureAddViewModel(
            IProcedureService procedureService,
            ISpecializationService specializationService,
            ISpecializationRequirementsProvider requirementsProvider)
        {
            _procedureService = procedureService;
            _specializationService = specializationService;
            _requirementsProvider = requirementsProvider;

            Title = "Dodaj procedurę";
            ExecutionDate = DateTime.Today;
            IsSelfPerformed = true;

            // Inicjalizacja kolekcji
            Categories = new ObservableCollection<string>();
            Stages = new ObservableCollection<string>();
            RequiredProcedures = new ObservableCollection<string>();
        }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private DateTime executionDate;

        [ObservableProperty]
        private bool isSelfPerformed;

        [ObservableProperty]
        private string location;

        [ObservableProperty]
        private string notes;

        [ObservableProperty]
        private string supervisorName;

        [ObservableProperty]
        private ObservableCollection<string> categories;

        [ObservableProperty]
        private string selectedCategory;

        [ObservableProperty]
        private ObservableCollection<string> stages;

        [ObservableProperty]
        private string selectedStage;

        [ObservableProperty]
        private ObservableCollection<string> requiredProcedures;

        [ObservableProperty]
        private string selectedRequiredProcedure;

        public bool CanSave =>
            !string.IsNullOrWhiteSpace(Name) &&
            !string.IsNullOrWhiteSpace(Location);

        private int _currentSpecializationId;

        public async Task InitializeAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var currentSpecialization = await _specializationService.GetCurrentSpecializationAsync();
                if (currentSpecialization != null)
                {
                    _currentSpecializationId = currentSpecialization.Id;
                }

                var procedureRequirements = _requirementsProvider.GetRequiredProceduresBySpecialization(_currentSpecializationId);

                RequiredProcedures.Clear();
                RequiredProcedures.Add("");

                foreach (var category in procedureRequirements.Keys)
                {
                    foreach (var procedure in procedureRequirements[category])
                    {
                        RequiredProcedures.Add(procedure.Name);
                    }
                }

                var availableCategories = await _procedureService.GetAvailableCategoriesAsync();
                Categories.Clear();

                Categories.Add("");

                foreach (var category in availableCategories)
                {
                    if (!string.IsNullOrEmpty(category))
                    {
                        Categories.Add(category);
                    }
                }

                var availableStages = await _procedureService.GetAvailableStagesAsync();
                Stages.Clear();
                Stages.Add("");

                foreach (var stage in availableStages)
                {
                    if (!string.IsNullOrEmpty(stage))
                    {
                        Stages.Add(stage);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Błąd",
                    $"Nie udało się załadować danych: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnSelectedRequiredProcedureChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Name = value;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Name))
                {
                    await Shell.Current.DisplayAlert(
                        "Błąd",
                        "Nazwa procedury jest wymagana.",
                        "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Location))
                {
                    await Shell.Current.DisplayAlert(
                        "Błąd",
                        "Miejsce wykonania jest wymagane.",
                        "OK");
                    return;
                }

                var procedure = new ProcedureExecution
                {
                    Name = Name,
                    ExecutionDate = ExecutionDate,
                    Type = IsSelfPerformed ? ProcedureType.Execution : ProcedureType.Assistance,
                    Location = Location,
                    Notes = Notes,
                    Category = SelectedCategory,
                    Stage = SelectedStage
                };

                if (!string.IsNullOrWhiteSpace(SupervisorName))
                {
                    procedure.Notes = string.IsNullOrEmpty(procedure.Notes)
                        ? $"Opiekun: {SupervisorName}"
                        : $"Opiekun: {SupervisorName}\n{procedure.Notes}";
                }

                await _procedureService.AddProcedureAsync(procedure);
                await Shell.Current.DisplayAlert(
                    "Sukces",
                    "Procedura została dodana pomyślnie.",
                    "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Błąd",
                    $"Nie udało się zapisać procedury: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
