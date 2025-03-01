using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Absences.ViewModels
{
    public partial class AbsenceDetailsViewModel : ViewModelBase
    {
        private readonly IDatabaseService _databaseService;
        private Action<Absence> _onSaveCallback;
        private Absence _absence;

        [ObservableProperty]
        private string _pageTitle;

        [ObservableProperty]
        private bool _isExistingAbsence;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        [ObservableProperty]
        private string _durationDays;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private bool _affectsSpecializationLength;

        [ObservableProperty]
        private string _documentReference;

        [ObservableProperty]
        private string _year;

        [ObservableProperty]
        private bool _isApproved;

        [ObservableProperty]
        private int _absenceTypeSelectedIndex;

        public AbsenceDetailsViewModel(
            IDatabaseService databaseService,
            ILogger<AbsenceDetailsViewModel> logger) : base(logger)
        {
            _databaseService = databaseService;
            Title = "Nieobecność";
        }

        public void Initialize(Absence absence, Action<Absence> onSaveCallback)
        {
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
                AbsenceTypeSelectedIndex = 0;
                Year = DateTime.Now.Year.ToString();
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
                AbsenceTypeSelectedIndex = (int)absence.Type;
            }
        }

        [RelayCommand]
        private void UpdateAbsenceType(int index)
        {
            _absence.Type = (AbsenceType)index;

            // Set default values based on type
            switch (_absence.Type)
            {
                case AbsenceType.SelfEducationLeave:
                    // Self-education leave typically affects specialization length
                    AffectsSpecializationLength = false;
                    break;
                case AbsenceType.SickLeave:
                case AbsenceType.MaternityLeave:
                case AbsenceType.ParentalLeave:
                    // These types typically extend the specialization
                    AffectsSpecializationLength = true;
                    break;
                case AbsenceType.VacationLeave:
                    // Vacation leave typically doesn't affect specialization length
                    AffectsSpecializationLength = false;
                    break;
            }
        }

        public void CalculateDuration()
        {
            if (EndDate >= StartDate)
            {
                int days = (EndDate - StartDate).Days + 1;
                DurationDays = days.ToString();
            }
            else
            {
                DurationDays = "0";
            }
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (!IsExistingAbsence)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Potwierdzenie",
                "Czy na pewno chcesz usunąć tę nieobecność?",
                "Tak",
                "Nie");

            if (confirm)
            {
                try
                {
                    await _databaseService.DeleteAsync(_absence);
                    await Shell.Current.Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting absence");
                    await Application.Current.MainPage.DisplayAlert(
                        "Błąd",
                        $"Nie udało się usunąć nieobecności: {ex.Message}",
                        "OK");
                }
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
            // Validation
            if (StartDate > EndDate)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Błąd",
                    "Data zakończenia musi być późniejsza lub równa dacie rozpoczęcia.",
                    "OK");
                return;
            }

            if (!int.TryParse(Year, out int year))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Błąd",
                    "Wprowadź poprawny rok.",
                    "OK");
                return;
            }

            // Upewnij się, że zawsze tworzysz NOWY obiekt Absence, nigdy nie modyfikuj istniejącego
            var absence = new Absence
            {
                StartDate = StartDate,
                EndDate = EndDate,
                DurationDays = int.Parse(DurationDays),
                Description = Description,
                AffectsSpecializationLength = AffectsSpecializationLength,
                DocumentReference = DocumentReference,
                Year = year,
                IsApproved = IsApproved,
                Type = (AbsenceType)AbsenceTypeSelectedIndex
            };

            if (_onSaveCallback != null)
            {
                _onSaveCallback(absence);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}