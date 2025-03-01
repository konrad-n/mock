using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Duties.ViewModels
{
    public partial class DutyShiftDetailsViewModel : ViewModelBase
    {
        private Func<DutyShift, Task> _onSaveCallback;
        private DutyShift _dutyShift;
        private bool _isNewDutyShift = false;

        [ObservableProperty]
        private string _pageTitle;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now;

        [ObservableProperty]
        private TimeSpan _startTime = new TimeSpan(8, 0, 0); // 8:00 AM

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private TimeSpan _endTime = new TimeSpan(8, 0, 0); // 8:00 AM next day

        [ObservableProperty]
        private string _durationText = "24 godziny";

        [ObservableProperty]
        private string _location = "";

        [ObservableProperty]
        private string _departmentName = ""; // For SMK

        [ObservableProperty]
        private string _supervisorName = "";

        [ObservableProperty]
        private string _notes = "";

        [ObservableProperty]
        private bool _isSupervisorVisible;

        [ObservableProperty]
        private int _dutyTypeSelectedIndex;

        public DutyShiftDetailsViewModel(ILogger<DutyShiftDetailsViewModel> logger) : base(logger)
        {
            Title = "Szczegóły dyżuru";
        }

        public void Initialize(DutyShift dutyShift, Func<DutyShift, Task> onSaveCallback)
        {
            _onSaveCallback = onSaveCallback;

            if (dutyShift == null)
            {
                // New duty shift
                _isNewDutyShift = true;
                _dutyShift = new DutyShift
                {
                    StartDate = DateTime.Now.Date.Add(StartTime),
                    EndDate = DateTime.Now.Date.AddDays(1).Add(EndTime),
                    DurationHours = 24,
                    Type = DutyType.Independent
                };
                PageTitle = "Dodaj dyżur";
                DutyTypeSelectedIndex = 0; // Independent
                IsSupervisorVisible = false;
            }
            else
            {
                // Edit existing duty shift
                _isNewDutyShift = false;
                _dutyShift = dutyShift;
                PageTitle = "Edytuj dyżur";

                StartDate = dutyShift.StartDate.Date;
                StartTime = dutyShift.StartDate.TimeOfDay;
                EndDate = dutyShift.EndDate.Date;
                EndTime = dutyShift.EndDate.TimeOfDay;

                Location = dutyShift.Location ?? "";
                DepartmentName = dutyShift.DepartmentName ?? "";
                SupervisorName = dutyShift.SupervisorName ?? "";
                Notes = dutyShift.Notes ?? "";

                DutyTypeSelectedIndex = dutyShift.Type == DutyType.Independent ? 0 : 1;
                IsSupervisorVisible = dutyShift.Type == DutyType.Accompanied;

                UpdateDurationText();
            }
        }

        [RelayCommand]
        private void UpdateDutyType(int index)
        {
            // Sprawdź czy _dutyShift nie jest null
            if (_dutyShift == null)
            {
                _logger?.LogWarning("Próba aktualizacji typu dyżuru gdy _dutyShift jest null");
                return;
            }

            _dutyShift.Type = index == 0 ? DutyType.Independent : DutyType.Accompanied;
            IsSupervisorVisible = _dutyShift.Type == DutyType.Accompanied;
        }

        public void UpdateDurationText()
        {
            try
            {
                // Sprawdź czy _dutyShift nie jest null
                if (_dutyShift == null)
                {
                    DurationText = "Wczytywanie...";
                    return;
                }

                var startDateTime = StartDate.Date.Add(StartTime);
                var endDateTime = EndDate.Date.Add(EndTime);

                if (endDateTime <= startDateTime)
                {
                    DurationText = "Nieprawidłowy zakres czasu";
                    return;
                }

                var duration = endDateTime - startDateTime;

                // Warn about long duty shifts
                if (duration.TotalHours > 24)
                {
                    MainThread.BeginInvokeOnMainThread(async () => {
                        bool continueAnyway = await Application.Current.MainPage.DisplayAlert(
                            "Uwaga",
                            "Dyżur przekracza 24 godziny. Dyżury powinny zazwyczaj trwać maksymalnie 24 godziny. Czy na pewno chcesz kontynuować?",
                            "Tak",
                            "Nie");

                        if (!continueAnyway)
                        {
                            // Reset to 24 hours duty
                            EndDate = StartDate.Date.AddDays(1);
                            EndTime = StartTime;
                            OnPropertyChanged(nameof(EndDate));
                            OnPropertyChanged(nameof(EndTime));
                            UpdateDurationText();
                            return;
                        }
                    });
                }

                _dutyShift.DurationHours = duration.TotalHours;

                // Format according to SMK requirements - split into hours and minutes
                int hours = (int)Math.Floor(duration.TotalHours);
                int minutes = (int)Math.Round((duration.TotalHours - hours) * 60);

                if (duration.TotalHours < 24)
                {
                    DurationText = $"{hours} godz. {minutes} min.";
                }
                else
                {
                    var days = Math.Floor(duration.TotalDays);
                    var remainingHours = hours - (days * 24);
                    DurationText = days > 0
                        ? $"{days} dni {remainingHours} godz. {minutes} min."
                        : $"{hours} godz. {minutes} min.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating duration text");
                DurationText = "Błąd obliczania czasu";
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(Location))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Miejsce dyżuru jest wymagane.", "OK");
                    return;
                }

                var startDateTime = StartDate.Date.Add(StartTime);
                var endDateTime = EndDate.Date.Add(EndTime);

                if (endDateTime <= startDateTime)
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Data i godzina zakończenia musi być późniejsza niż data i godzina rozpoczęcia.", "OK");
                    return;
                }

                // For accompanied type, supervisor is required
                if (_dutyShift.Type == DutyType.Accompanied && string.IsNullOrWhiteSpace(SupervisorName))
                {
                    await Application.Current.MainPage.DisplayAlert("Błąd", "Imię i nazwisko nadzorującego jest wymagane dla dyżuru towarzyszącego.", "OK");
                    return;
                }

                // Always create a new duty shift object to avoid reference issues
                var dutyShiftToSave = new DutyShift
                {
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    DurationHours = (endDateTime - startDateTime).TotalHours,
                    Location = Location,
                    DepartmentName = DepartmentName,
                    SupervisorName = SupervisorName,
                    Notes = Notes,
                    Type = _dutyShift.Type
                };

                // Only set the ID if we're editing an existing duty shift
                if (!_isNewDutyShift)
                {
                    dutyShiftToSave.Id = _dutyShift.Id;
                }

                if (_onSaveCallback != null)
                {
                    await _onSaveCallback(dutyShiftToSave);
                }
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving duty shift");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił problem podczas zapisywania dyżuru: {ex.Message}", "OK");
            }
        }
    }
}