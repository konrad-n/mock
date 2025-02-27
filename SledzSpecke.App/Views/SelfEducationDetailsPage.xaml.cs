using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Views
{
    public partial class SelfEducationDetailsPage : ContentPage
    {
        private SelfEducation _selfEducation;
        private Action<SelfEducation> _onSaveCallback;
        public string PageTitle { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
        public string DurationDays { get; set; } = "1";
        public string Location { get; set; }
        public string Organizer { get; set; }
        public bool IsRequired { get; set; }
        public string Notes { get; set; }

        public SelfEducationDetailsPage(SelfEducation selfEducation, Action<SelfEducation> onSaveCallback)
        {
            InitializeComponent();
            _onSaveCallback = onSaveCallback;

            if (selfEducation == null)
            {
                // Nowe wydarzenie
                _selfEducation = new SelfEducation
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    StartDate = StartDate,
                    EndDate = EndDate,
                    DurationDays = 1,
                    Type = SelfEducationType.Conference
                };
                PageTitle = "Dodaj wydarzenie";
                TypePicker.SelectedIndex = 0; // Konferencja
            }
            else
            {
                // Edycja istniejącego wydarzenia
                _selfEducation = selfEducation;
                PageTitle = "Edytuj wydarzenie";

                Title = selfEducation.Title;
                StartDate = selfEducation.StartDate;
                EndDate = selfEducation.EndDate;
                DurationDays = selfEducation.DurationDays.ToString();
                Location = selfEducation.Location;
                Organizer = selfEducation.Organizer;
                IsRequired = selfEducation.IsRequired;
                Notes = selfEducation.Notes;

                TypePicker.SelectedIndex = (int)selfEducation.Type;
            }

            BindingContext = this;
        }

        private void OnTypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            _selfEducation.Type = (SelfEducationType)TypePicker.SelectedIndex;
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            UpdateDurationDays();
        }

        private void UpdateDurationDays()
        {
            if (EndDate >= StartDate)
            {
                TimeSpan duration = EndDate - StartDate;
                DurationDays = (duration.Days + 1).ToString();
                OnPropertyChanged(nameof(DurationDays));
            }
        }

        private async void OnAddAttachmentClicked(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await FilePicker.PickAsync();
                if (fileResult != null)
                {
                    _selfEducation.CertificateFilePath = fileResult.FullPath;
                    await DisplayAlert("Sukces", "Plik został dodany pomyślnie.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił problem z wyborem pliku: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(Title))
            {
                await DisplayAlert("Błąd", "Tytuł wydarzenia jest wymagany.", "OK");
                return;
            }

            if (EndDate < StartDate)
            {
                await DisplayAlert("Błąd", "Data zakończenia musi być późniejsza lub równa dacie rozpoczęcia.", "OK");
                return;
            }

            if (!int.TryParse(DurationDays, out int durationDays) || durationDays <= 0)
            {
                await DisplayAlert("Błąd", "Wprowadź poprawną liczbę dni.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Location))
            {
                await DisplayAlert("Błąd", "Miejsce wydarzenia jest wymagane.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Organizer))
            {
                await DisplayAlert("Błąd", "Nazwa organizatora jest wymagana.", "OK");
                return;
            }

            // Aktualizacja wydarzenia
            _selfEducation.Title = Title;
            _selfEducation.StartDate = StartDate;
            _selfEducation.EndDate = EndDate;
            _selfEducation.DurationDays = durationDays;
            _selfEducation.Location = Location;
            _selfEducation.Organizer = Organizer;
            _selfEducation.IsRequired = IsRequired;
            _selfEducation.Notes = Notes;

            _onSaveCallback?.Invoke(_selfEducation);
            await Navigation.PopAsync();
        }
    }
}