using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Duties.Views
{
    public partial class DutyShiftDetailsPage : ContentPage
    {
        private DutyShift _dutyShift;
        private Func<DutyShift, Task> _onSaveCallback;
        private bool _isNewDutyShift = false;

        public string PageTitle { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
        public TimeSpan EndTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM next day
        public string DurationText { get; set; } = "24 godziny";
        public string Location { get; set; } = "";
        public string DepartmentName { get; set; } = ""; // New field for SMK
        public string SupervisorName { get; set; } = "";
        public string Notes { get; set; } = "";
        public bool IsSupervisorVisible { get; set; }

        public DutyShiftDetailsPage(DutyShift dutyShift, Func<DutyShift, Task> onSaveCallback)
        {
            InitializeComponent();
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
                DutyTypePicker.SelectedIndex = 0; // Independent
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

                DutyTypePicker.SelectedIndex = dutyShift.Type == DutyType.Independent ? 0 : 1;
                IsSupervisorVisible = dutyShift.Type == DutyType.Accompanied;

                UpdateDurationText();
            }

            BindingContext = this;
        }

        private void OnDutyTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            _dutyShift.Type = DutyTypePicker.SelectedIndex == 0 ? DutyType.Independent : DutyType.Accompanied;
            IsSupervisorVisible = _dutyShift.Type == DutyType.Accompanied;
            OnPropertyChanged(nameof(IsSupervisorVisible));
        }

        private void OnDateTimeChanged(object sender, EventArgs e)
        {
            UpdateDurationText();
        }

        private void UpdateDurationText()
        {
            try
            {
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
                        bool continueAnyway = await DisplayAlert(
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

                OnPropertyChanged(nameof(DurationText));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating duration text: {ex.Message}");
                DurationText = "Błąd obliczania czasu";
                OnPropertyChanged(nameof(DurationText));
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(Location))
                {
                    await DisplayAlert("Błąd", "Miejsce dyżuru jest wymagane.", "OK");
                    return;
                }

                var startDateTime = StartDate.Date.Add(StartTime);
                var endDateTime = EndDate.Date.Add(EndTime);

                if (endDateTime <= startDateTime)
                {
                    await DisplayAlert("Błąd", "Data i godzina zakończenia musi być późniejsza niż data i godzina rozpoczęcia.", "OK");
                    return;
                }

                // For accompanied type, supervisor is required
                if (_dutyShift.Type == DutyType.Accompanied && string.IsNullOrWhiteSpace(SupervisorName))
                {
                    await DisplayAlert("Błąd", "Imię i nazwisko nadzorującego jest wymagane dla dyżuru towarzyszącego.", "OK");
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
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem podczas zapisywania dyżuru: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error saving duty shift: {ex}");
            }
        }
    }
}