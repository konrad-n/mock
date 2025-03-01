using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Absences.Views
{
    public partial class AbsenceDetailsPage : ContentPage
    {
        private Absence _absence;
        private IDatabaseService _databaseService;
        private Action<Absence> _onSaveCallback;

        public string PageTitle { get; set; }
        public bool IsExistingAbsence { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public string DurationDays { get; set; }
        public string Description { get; set; }
        public bool AffectsSpecializationLength { get; set; }
        public string DocumentReference { get; set; }
        public string Year { get; set; }
        public bool IsApproved { get; set; }

        public AbsenceDetailsPage(Absence absence, Action<Absence> onSaveCallback)
        {
            InitializeComponent();

            _databaseService = App.DatabaseService;

            _onSaveCallback = onSaveCallback;

            if (absence == null)
            {
                // Nowa nieobecność
                _absence = new Absence
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Type = AbsenceType.SickLeave,
                    Year = DateTime.Now.Year,
                    AffectsSpecializationLength = true
                };
                PageTitle = "Dodaj nieobecność";
                IsExistingAbsence = false;
                AbsenceTypePicker.SelectedIndex = 0;
                CalculateDuration();
            }
            else
            {
                // Edycja istniejącej nieobecności
                _absence = absence;
                PageTitle = "Edytuj nieobecność";
                IsExistingAbsence = true;

                StartDate = absence.StartDate;
                EndDate = absence.EndDate;
                DurationDays = absence.DurationDays.ToString();
                Description = absence.Description;
                AffectsSpecializationLength = absence.AffectsSpecializationLength;
                DocumentReference = absence.DocumentReference;
                Year = absence.Year.ToString();
                IsApproved = absence.IsApproved;

                // Set absence type picker
                AbsenceTypePicker.SelectedIndex = (int)absence.Type;
            }

            BindingContext = this;
        }

        private void OnAbsenceTypeChanged(object sender, EventArgs e)
        {
            _absence.Type = (AbsenceType)AbsenceTypePicker.SelectedIndex;

            // Set default values based on type
            switch (_absence.Type)
            {
                case AbsenceType.SelfEducationLeave:
                    // Self-education leave typically affects specialization length
                    AffectsSpecializationLength = false;
                    OnPropertyChanged(nameof(AffectsSpecializationLength));
                    break;
                case AbsenceType.SickLeave:
                case AbsenceType.MaternityLeave:
                case AbsenceType.ParentalLeave:
                    // These types typically extend the specialization
                    AffectsSpecializationLength = true;
                    OnPropertyChanged(nameof(AffectsSpecializationLength));
                    break;
                case AbsenceType.VacationLeave:
                    // Vacation leave typically doesn't affect specialization length
                    AffectsSpecializationLength = false;
                    OnPropertyChanged(nameof(AffectsSpecializationLength));
                    break;
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            CalculateDuration();
        }

        private void CalculateDuration()
        {
            if (EndDate >= StartDate)
            {
                int days = (EndDate - StartDate).Days + 1;
                DurationDays = days.ToString();
                OnPropertyChanged(nameof(DurationDays));
            }
            else
            {
                DurationDays = "0";
                OnPropertyChanged(nameof(DurationDays));
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Potwierdzenie", "Czy na pewno chcesz usunąć tę nieobecność?", "Tak", "Nie");
            if (confirm)
            {
                try
                {
                    await _databaseService.DeleteAsync(_absence);
                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Błąd", $"Nie udało się usunąć nieobecności: {ex.Message}", "OK");
                }
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validation
            if (StartDate > EndDate)
            {
                await DisplayAlert("Błąd", "Data zakończenia musi być późniejsza lub równa dacie rozpoczęcia.", "OK");
                return;
            }

            if (!int.TryParse(Year, out int year))
            {
                await DisplayAlert("Błąd", "Wprowadź poprawny rok.", "OK");
                return;
            }

            // Update absence
            _absence.StartDate = StartDate;
            _absence.EndDate = EndDate;
            _absence.DurationDays = int.Parse(DurationDays);
            _absence.Description = Description;
            _absence.AffectsSpecializationLength = AffectsSpecializationLength;
            _absence.DocumentReference = DocumentReference;
            _absence.Year = year;
            _absence.IsApproved = IsApproved;
            _absence.Type = (AbsenceType)AbsenceTypePicker.SelectedIndex;

            if (_onSaveCallback != null)
            {
                _onSaveCallback(_absence);
            }

            await Navigation.PopAsync();
        }
    }
}