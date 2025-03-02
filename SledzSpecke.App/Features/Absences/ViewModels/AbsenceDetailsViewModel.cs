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
            this._databaseService = databaseService;
            this.Title = "Nieobecność";
        }

        public void Initialize(Absence absence, Action<Absence> onSaveCallback)
        {
            this._onSaveCallback = onSaveCallback;

            if (absence == null)
            {
                // Nowa nieobecność
                this._absence = new Absence
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Type = AbsenceType.SickLeave,
                    Year = DateTime.Now.Year,
                    AffectsSpecializationLength = true
                };
                this.PageTitle = "Dodaj nieobecność";
                this.IsExistingAbsence = false;
                this.AbsenceTypeSelectedIndex = 0;
                this.Year = DateTime.Now.Year.ToString();
                this.CalculateDuration();
            }
            else
            {
                // Edycja istniejącej nieobecności
                this._absence = absence;
                this.PageTitle = "Edytuj nieobecność";
                this.IsExistingAbsence = true;

                this.StartDate = absence.StartDate;
                this.EndDate = absence.EndDate;
                this.DurationDays = absence.DurationDays.ToString();
                this.Description = absence.Description;
                this.AffectsSpecializationLength = absence.AffectsSpecializationLength;
                this.DocumentReference = absence.DocumentReference;
                this.Year = absence.Year.ToString();
                this.IsApproved = absence.IsApproved;

                // Set absence type picker
                this.AbsenceTypeSelectedIndex = (int)absence.Type;
            }
        }

        [RelayCommand]
        private void UpdateAbsenceType(int index)
        {
            this._absence.Type = (AbsenceType)index;

            // Set default values based on type
            switch (this._absence.Type)
            {
                case AbsenceType.SelfEducationLeave:
                    // Self-education leave typically affects specialization length
                    this.AffectsSpecializationLength = false;
                    break;
                case AbsenceType.SickLeave:
                case AbsenceType.MaternityLeave:
                case AbsenceType.ParentalLeave:
                    // These types typically extend the specialization
                    this.AffectsSpecializationLength = true;
                    break;
                case AbsenceType.VacationLeave:
                    // Vacation leave typically doesn't affect specialization length
                    this.AffectsSpecializationLength = false;
                    break;
            }
        }

        public void CalculateDuration()
        {
            if (this.EndDate >= this.StartDate)
            {
                int days = (this.EndDate - this.StartDate).Days + 1;
                this.DurationDays = days.ToString();
            }
            else
            {
                this.DurationDays = "0";
            }
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (!this.IsExistingAbsence)
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
                    await this._databaseService.DeleteAsync(this._absence);
                    await Shell.Current.Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Error deleting absence");
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
            if (this.StartDate > this.EndDate)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Błąd",
                    "Data zakończenia musi być późniejsza lub równa dacie rozpoczęcia.",
                    "OK");
                return;
            }

            if (!int.TryParse(this.Year, out int year))
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
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                DurationDays = int.Parse(this.DurationDays),
                Description = this.Description,
                AffectsSpecializationLength = this.AffectsSpecializationLength,
                DocumentReference = this.DocumentReference,
                Year = year,
                IsApproved = this.IsApproved,
                Type = (AbsenceType)this.AbsenceTypeSelectedIndex
            };

            if (this._onSaveCallback != null)
            {
                this._onSaveCallback(absence);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}