using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.ViewModels.Duties;

public partial class DutyEditViewModel : BaseViewModel
{
    private readonly IDutyService _dutyService;
    private int _dutyId;

    public DutyEditViewModel(IDutyService dutyService)
    {
        _dutyService = dutyService;
        Title = "Edytuj dyżur";
        DutyTypes = Enum.GetValues(typeof(DutyType))
            .Cast<DutyType>()
            .ToList();
    }

    [ObservableProperty]
    private string location;

    [ObservableProperty]
    private DateTime startDate;

    [ObservableProperty]
    private DateTime endDate;

    [ObservableProperty]
    private TimeSpan startTime;

    [ObservableProperty]
    private TimeSpan endTime;

    [ObservableProperty]
    private List<DutyType> dutyTypes;

    [ObservableProperty]
    private DutyType selectedDutyType;

    [ObservableProperty]
    private string notes;

    public bool CanSave =>
        !string.IsNullOrWhiteSpace(Location) &&
        GetStartDateTime() < GetEndDateTime();

    private DateTime GetStartDateTime() =>
        StartDate.Date + StartTime;

    private DateTime GetEndDateTime() =>
        EndDate.Date + EndTime;

    public async Task LoadDuty(int id)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            _dutyId = id;

            var duty = await _dutyService.GetDutyAsync(id);
            if (duty != null)
            {
                Location = duty.Location;
                StartDate = duty.StartTime.Date;
                StartTime = duty.StartTime.TimeOfDay;
                EndDate = duty.EndTime.Date;
                EndTime = duty.EndTime.TimeOfDay;
                SelectedDutyType = duty.Type;
                Notes = duty.Notes;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się załadować danych dyżuru",
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

            var duty = new Duty
            {
                Id = _dutyId,
                Location = Location,
                StartTime = GetStartDateTime(),
                EndTime = GetEndDateTime(),
                Type = SelectedDutyType,
                Notes = Notes
            };

            await _dutyService.UpdateDutyAsync(duty);
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
            "Czy na pewno chcesz usunąć ten dyżur?",
            "Usuń",
            "Anuluj");

        if (!shouldDelete) return;

        try
        {
            IsBusy = true;
            await _dutyService.DeleteDutyAsync(_dutyId);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się usunąć dyżuru",
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