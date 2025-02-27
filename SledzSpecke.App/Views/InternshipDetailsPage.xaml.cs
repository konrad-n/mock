using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Views
{
    public partial class InternshipDetailsPage : ContentPage
    {
        private Internship _internship;
        private ModuleType _currentModule;
        private Action<Internship> _onSaveCallback;
        public string PageTitle { get; set; }
        public string DurationWeeks { get; set; }
        public string WorkingDays { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);
        public bool IsStartDateVisible { get; set; }
        public bool IsEndDateVisible { get; set; }
        public bool IsCompletionVisible { get; set; }

        public Internship Internship => _internship;

        public InternshipDetailsPage(Internship internship, ModuleType currentModule, Action<Internship> onSaveCallback)
        {
            InitializeComponent();
            _currentModule = currentModule;
            _onSaveCallback = onSaveCallback;

            if (internship == null)
            {
                // Nowy staż
                _internship = new Internship
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    IsRequired = true
                };
                PageTitle = "Dodaj staż";
            }
            else
            {
                // Edycja istniejącego stażu
                _internship = internship;
                PageTitle = "Szczegóły stażu";
                DurationWeeks = internship.DurationWeeks.ToString();
                WorkingDays = internship.WorkingDays.ToString();

                // Ustawienie pickerów
                ModulePicker.SelectedIndex = internship.Module == ModuleType.Basic ? 0 : 1;

                // Ustawienie statusu
                if (internship.IsCompleted)
                {
                    StatusPicker.SelectedIndex = 3; // Ukończony
                    IsStartDateVisible = true;
                    IsEndDateVisible = true;
                    IsCompletionVisible = true;
                    if (internship.StartDate.HasValue)
                        StartDate = internship.StartDate.Value;
                    if (internship.EndDate.HasValue)
                        EndDate = internship.EndDate.Value;
                }
                else if (internship.StartDate.HasValue && !internship.EndDate.HasValue)
                {
                    StatusPicker.SelectedIndex = 2; // W trakcie
                    IsStartDateVisible = true;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    StartDate = internship.StartDate.Value;
                }
                else if (internship.StartDate.HasValue)
                {
                    StatusPicker.SelectedIndex = 1; // Zaplanowany
                    IsStartDateVisible = true;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    StartDate = internship.StartDate.Value;
                }
                else
                {
                    StatusPicker.SelectedIndex = 0; // Oczekujący
                    IsStartDateVisible = false;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                }
            }

            BindingContext = this;
        }

        private void OnModulePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            _internship.Module = ModulePicker.SelectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
        }

        private void OnStatusPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (StatusPicker.SelectedIndex)
            {
                case 0: // Oczekujący
                    IsStartDateVisible = false;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    _internship.StartDate = null;
                    _internship.EndDate = null;
                    _internship.IsCompleted = false;
                    break;
                case 1: // Zaplanowany
                    IsStartDateVisible = true;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    _internship.StartDate = StartDate;
                    _internship.EndDate = null;
                    _internship.IsCompleted = false;
                    break;
                case 2: // W trakcie
                    IsStartDateVisible = true;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    _internship.StartDate = StartDate;
                    _internship.EndDate = null;
                    _internship.IsCompleted = false;
                    break;
                case 3: // Ukończony
                    IsStartDateVisible = true;
                    IsEndDateVisible = true;
                    IsCompletionVisible = true;
                    _internship.StartDate = StartDate;
                    _internship.EndDate = EndDate;
                    _internship.IsCompleted = true;
                    break;
            }
            OnPropertyChanged(nameof(IsStartDateVisible));
            OnPropertyChanged(nameof(IsEndDateVisible));
            OnPropertyChanged(nameof(IsCompletionVisible));
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(_internship.Name))
            {
                await DisplayAlert("Błąd", "Nazwa stażu jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(DurationWeeks, out int durationWeeks) || durationWeeks <= 0)
            {
                await DisplayAlert("Błąd", "Wprowadź poprawny czas trwania stażu (tygodnie).", "OK");
                return;
            }

            if (!int.TryParse(WorkingDays, out int workingDays) || workingDays <= 0)
            {
                await DisplayAlert("Błąd", "Wprowadź poprawną liczbę dni roboczych.", "OK");
                return;
            }

            _internship.DurationWeeks = durationWeeks;
            _internship.WorkingDays = workingDays;

            // Dodatkowe ustawienia w zależności od statusu
            if (IsStartDateVisible && _internship.StartDate == null)
            {
                _internship.StartDate = StartDate;
            }

            if (IsEndDateVisible && _internship.EndDate == null)
            {
                _internship.EndDate = EndDate;
            }

            _onSaveCallback?.Invoke(_internship);
            await Navigation.PopAsync();
        }
    }
}