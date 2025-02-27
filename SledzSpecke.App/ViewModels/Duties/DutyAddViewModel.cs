using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;

namespace SledzSpecke.App.ViewModels.Duties;

public partial class DutyAddViewModel : BaseViewModel
{
    private readonly IDutyService _dutyService;

    public DutyAddViewModel(IDutyService dutyService)
    {
        _dutyService = dutyService;
        Title = "Dodaj dyżur";
        StartDate = DateTime.Today;
        EndDate = DateTime.Today.AddDays(1);
        StartTime = TimeSpan.FromHours(8);
        EndTime = TimeSpan.FromHours(8);
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
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            if (StartDate == EndDate)
            {
                StartTime = new TimeSpan(8, 0, 0);
                EndTime = new TimeSpan(8, 0, 0);
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

        if (string.IsNullOrWhiteSpace(Location))
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Miejsce dyżuru jest wymagane.",
                "OK");
            return;
        }

        if (GetStartDateTime() >= GetEndDateTime())
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Data zakończenia musi być późniejsza niż data rozpoczęcia.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var duty = new Duty
            {
                Location = Location,
                StartTime = GetStartDateTime(),
                EndTime = GetEndDateTime(),
                Notes = Notes,
                DurationInHours = (decimal)(GetEndDateTime() - GetStartDateTime()).TotalHours
            };

            await _dutyService.AddDutyAsync(duty);
            await Shell.Current.DisplayAlert(
                "Sukces",
                "Dyżur został dodany pomyślnie.",
                "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                $"Nie udało się zapisać dyżuru: {ex.Message}",
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
