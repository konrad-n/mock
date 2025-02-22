using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.ViewModels.Procedures;

public partial class ProcedureEditViewModel : BaseViewModel
{
    private readonly IProcedureService _procedureService;
    private int _procedureId;

    public ProcedureEditViewModel(IProcedureService procedureService)
    {
        _procedureService = procedureService;
        Title = "Edytuj procedur?";
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

    public async Task LoadProcedure(int id)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            _procedureId = id;

            var procedure = await _procedureService.GetProcedureAsync(id);
            if (procedure != null)
            {
                Name = procedure.Name;
                ExecutionDate = procedure.ExecutionDate;
                IsSelfPerformed = procedure.Type == ProcedureType.Execution;
                Location = procedure.Location;
                Notes = procedure.Notes;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
            "B??d",
                "Nie uda?o si? za?adowa? danych procedury",
                "OK");
            await Shell.Current.GoToAsync("..");
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
                Id = _procedureId,
                Name = Name,
                ExecutionDate = ExecutionDate,
                Type = IsSelfPerformed ? ProcedureType.Execution : ProcedureType.Assistance,
                Location = Location,
                Notes = Notes
            };

            await _procedureService.UpdateProcedureAsync(procedure);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "B??d",
                "Nie uda?o si? zapisa? zmian",
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
            "Czy na pewno chcesz usun?? t? procedur??",
            "Usu?",
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
                "B??d",
                "Nie uda?o si? usun?? procedury",
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