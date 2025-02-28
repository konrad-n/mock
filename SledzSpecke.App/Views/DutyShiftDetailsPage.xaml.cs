using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Views
{
    public partial class DutyShiftDetailsPage : ContentPage
    {
        private DutyShift _dutyShift;
        private Func<DutyShift, Task> _onSaveCallback;

        public string PageTitle { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
        public TimeSpan EndTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM next day
        public string DurationText { get; set; } = "24 godziny";
        public string Location { get; set; }
        public string SupervisorName { get; set; }
        public string Notes { get; set; }
        public bool IsSupervisorVisible { get; set; }

        public DutyShiftDetailsPage(DutyShift dutyShift, Func<DutyShift, Task> onSaveCallback)
        {
            InitializeComponent();
            _onSaveCallback = onSaveCallback;

            if (dutyShift == null)
            {
                // New duty shift
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
                _dutyShift = dutyShift;
                PageTitle = "Edytuj dyżur";

                StartDate = dutyShift.StartDate.Date;
                StartTime = dutyShift.StartDate.TimeOfDay;
                EndDate = dutyShift.EndDate.Date;
                EndTime = dutyShift.EndDate.TimeOfDay;

                Location = dutyShift.Location;
                SupervisorName = dutyShift.SupervisorName;
                Notes = dutyShift.Notes;

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
            var startDateTime = StartDate.Date.Add(StartTime);
            var endDateTime = EndDate.Date.Add(EndTime);

            if (endDateTime <= startDateTime)
            {
                DurationText = "Nieprawidłowy zakres czasu";
                return;
            }

            var duration = endDateTime - startDateTime;
            _dutyShift.DurationHours = duration.TotalHours;

            if (duration.TotalHours < 24)
            {
                DurationText = $"{duration.Hours} godz. {duration.Minutes} min.";
            }
            else
            {
                var days = Math.Floor(duration.TotalDays);
                var remainingHours = duration.Hours - (days * 24);
                DurationText = days > 0
                    ? $"{days} dni {remainingHours} godz. {duration.Minutes} min."
                    : $"{duration.Hours} godz. {duration.Minutes} min.";
            }

            OnPropertyChanged(nameof(DurationText));
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
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

            // Update duty shift
            _dutyShift.StartDate = startDateTime;
            _dutyShift.EndDate = endDateTime;
            _dutyShift.DurationHours = (endDateTime - startDateTime).TotalHours;
            _dutyShift.Location = Location;
            _dutyShift.SupervisorName = SupervisorName;
            _dutyShift.Notes = Notes;

            try
            {
                if (_onSaveCallback != null)
                {
                    await _onSaveCallback(_dutyShift);
                }
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save duty shift: {ex.Message}", "OK");
            }
        }
    }
}