using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;

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

    public bool CanSave =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(Location);

    public async Task InitializeAsync()
    {
        // Można tu dodać ładowanie dodatkowych danych
        // np. lista dostępnych lokalizacji
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
                Notes = Notes
            };

            await _procedureService.AddProcedureAsync(procedure);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się zapisać procedury",
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