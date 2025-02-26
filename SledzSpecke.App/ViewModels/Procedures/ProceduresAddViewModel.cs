using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;
using System.Collections.ObjectModel;
using static Microsoft.Maui.Controls.Device;

namespace SledzSpecke.App.ViewModels.Procedures;

public partial class ProcedureAddViewModel : BaseViewModel
{
    private readonly IProcedureService _procedureService;

    public ProcedureAddViewModel(IProcedureService procedureService)
    {
        _procedureService = procedureService;
        Title = "Dodaj procedurę";
        ExecutionDate = DateTime.Today;
        IsSelfPerformed = true;

        // Inicjalizacja kolekcji
        Categories = new ObservableCollection<string>();
        Stages = new ObservableCollection<string>();
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

    public bool CanSave =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(Location);

    public async Task InitializeAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // Pobierz dostępne kategorie
            var availableCategories = await _procedureService.GetAvailableCategoriesAsync();
            Categories.Clear();

            // Dodaj pustą opcję na początku
            Categories.Add("");

            foreach (var category in availableCategories)
            {
                if (!string.IsNullOrEmpty(category))
                {
                    Categories.Add(category);
                }
            }

            // Pobierz dostępne etapy
            var availableStages = await _procedureService.GetAvailableStagesAsync();
            Stages.Clear();

            // Dodaj pustą opcję na początku
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

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

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

            // Jeśli podano nazwę opiekuna, możemy zaimplementować logikę do powiązania jej z istniejącym 
            // użytkownikiem w systemie lub po prostu zachować ją jako notatkę
            if (!string.IsNullOrWhiteSpace(SupervisorName))
            {
                procedure.Notes = string.IsNullOrEmpty(procedure.Notes)
                    ? $"Opiekun: {SupervisorName}"
                    : $"Opiekun: {SupervisorName}\n{procedure.Notes}";
            }

            await _procedureService.AddProcedureAsync(procedure);
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
