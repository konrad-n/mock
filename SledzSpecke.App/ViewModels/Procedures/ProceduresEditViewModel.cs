using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.ViewModels.Procedures;

public partial class ProcedureEditViewModel : BaseViewModel
{
    private readonly IProcedureService _procedureService;
    private int _procedureId;

    public ProcedureEditViewModel(IProcedureService procedureService)
    {
        _procedureService = procedureService;
        Title = "Edytuj procedurę";

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

    public async Task LoadProcedure(int id)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            _procedureId = id;

            await LoadCategoriesAndStagesAsync();

            var procedure = await _procedureService.GetProcedureAsync(id);
            if (procedure != null)
            {
                Name = procedure.Name;
                ExecutionDate = procedure.ExecutionDate;
                IsSelfPerformed = procedure.Type == ProcedureType.Execution;
                Location = procedure.Location;
                Notes = procedure.Notes;
                SelectedCategory = procedure.Category ?? "";
                SelectedStage = procedure.Stage ?? "";

                ExtractSupervisorFromNotes();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się załadować danych procedury",
                "OK");
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ExtractSupervisorFromNotes()
    {
        if (string.IsNullOrEmpty(Notes)) return;

        const string supervisorPrefix = "Opiekun: ";
        var lines = Notes.Split('\n');

        foreach (var line in lines)
        {
            if (line.StartsWith(supervisorPrefix))
            {
                SupervisorName = line.Substring(supervisorPrefix.Length).Trim();
                Notes = string.Join('\n', lines.Where(l => !l.StartsWith(supervisorPrefix)));
                break;
            }
        }
    }

    private async Task LoadCategoriesAndStagesAsync()
    {
        try
        {
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
            System.Diagnostics.Debug.WriteLine($"Błąd ładowania kategorii i etapów: {ex.Message}");
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
                Id = _procedureId,
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

            await _procedureService.UpdateProcedureAsync(procedure);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się zapisać zmian",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (IsBusy) return;

        var shouldDelete = await Shell.Current.DisplayAlert(
            "Potwierdzenie",
            "Czy na pewno chcesz usunąć tę procedurę?",
            "Usuń",
            "Anuluj");

        if (!shouldDelete) return;

        try
        {
            IsBusy = true;
            await _procedureService.DeleteProcedureAsync(_procedureId);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się usunąć procedury",
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
