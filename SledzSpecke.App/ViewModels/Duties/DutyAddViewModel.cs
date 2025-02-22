using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.ViewModels.Duties;

public partial class DutyAddViewModel : BaseViewModel
{
    private readonly IDutyService _dutyService;

    public DutyAddViewModel(IDutyService dutyService)
    {
        _dutyService = dutyService;
        Title = "Dodaj dyżur";
        StartDate = DateTime.Today;
        EndDate = DateTime.Today;
        StartTime = TimeSpan.FromHours(8);
        EndTime = TimeSpan.FromHours(8);
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

    public async Task InitializeAsync()
    {
        // Można tu załadować np. listę dostępnych lokalizacji
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
                Location = Location,
                StartTime = GetStartDateTime(),
                EndTime = GetEndDateTime(),
                Type = SelectedDutyType,
                Notes = Notes
            };

            await _dutyService.AddDutyAsync(duty);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się zapisać dyżuru",
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