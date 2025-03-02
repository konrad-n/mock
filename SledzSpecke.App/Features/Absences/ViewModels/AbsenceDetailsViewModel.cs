// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceDetailsViewModel.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   ViewModel szczegółów nieobecności.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Absences.ViewModels
{
    /// <summary>
    /// ViewModel szczegółów nieobecności.
    /// </summary>
    public partial class AbsenceDetailsViewModel : ViewModelBase
    {
        private readonly IDatabaseService databaseService;
        private Action<Absence>? onSaveCallback;
        private Absence? absence;

        /// <summary>
        /// Tytuł strony.
        /// </summary>
        [ObservableProperty]
        private string _pageTitle = string.Empty;

        /// <summary>
        /// Określa, czy nieobecność już istnieje.
        /// </summary>
        [ObservableProperty]
        private bool _isExistingAbsence;

        /// <summary>
        /// Data rozpoczęcia nieobecności.
        /// </summary>
        [ObservableProperty]
        private DateTime _startDate = DateTime.Now;

        /// <summary>
        /// Data zakończenia nieobecności.
        /// </summary>
        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        /// <summary>
        /// Liczba dni nieobecności.
        /// </summary>
        [ObservableProperty]
        private string _durationDays = string.Empty;

        /// <summary>
        /// Opis nieobecności.
        /// </summary>
        [ObservableProperty]
        private string _description = string.Empty;

        /// <summary>
        /// Określa, czy nieobecność wydłuża czas trwania specjalizacji.
        /// </summary>
        [ObservableProperty]
        private bool _affectsSpecializationLength;

        /// <summary>
        /// Sygnatura dokumentu.
        /// </summary>
        [ObservableProperty]
        private string _documentReference = string.Empty;

        /// <summary>
        /// Rok nieobecności.
        /// </summary>
        [ObservableProperty]
        private string _year = string.Empty;

        /// <summary>
        /// Określa, czy nieobecność jest zatwierdzona.
        /// </summary>
        [ObservableProperty]
        private bool _isApproved;

        /// <summary>
        /// Indeks wybranego typu nieobecności.
        /// </summary>
        [ObservableProperty]
        private int _absenceTypeSelectedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbsenceDetailsViewModel"/> class.
        /// </summary>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <param name="logger">Logger.</param>
        public AbsenceDetailsViewModel(
            IDatabaseService databaseService,
            ILogger<AbsenceDetailsViewModel> logger)
            : base(logger)
        {
            this.databaseService = databaseService;
            this.Title = "Nieobecność";
        }

        /// <summary>
        /// Inicjalizuje ViewModel.
        /// </summary>
        /// <param name="absenceParam">Nieobecność do edycji lub null dla nowej nieobecności.</param>
        /// <param name="saveCallback">Wywołanie zwrotne po zapisaniu nieobecności.</param>
        public void Initialize(Absence? absenceParam, Action<Absence> saveCallback)
        {
            this.onSaveCallback = saveCallback;

            if (absenceParam == null)
            {
                // Nowa nieobecność
                this.absence = new Absence
                {
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Type = AbsenceType.SickLeave,
                    Year = DateTime.Now.Year,
                    AffectsSpecializationLength = true,
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
                this.absence = absenceParam;
                this.PageTitle = "Edytuj nieobecność";
                this.IsExistingAbsence = true;

                this.StartDate = absenceParam.StartDate;
                this.EndDate = absenceParam.EndDate;
                this.DurationDays = absenceParam.DurationDays.ToString();
                this.Description = absenceParam.Description ?? string.Empty;
                this.AffectsSpecializationLength = absenceParam.AffectsSpecializationLength;
                this.DocumentReference = absenceParam.DocumentReference ?? string.Empty;
                this.Year = absenceParam.Year.ToString();
                this.IsApproved = absenceParam.IsApproved;

                // Set absence type picker
                this.AbsenceTypeSelectedIndex = (int)absenceParam.Type;
            }
        }

        /// <summary>
        /// Oblicza liczbę dni nieobecności.
        /// </summary>
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

        /// <summary>
        /// Anuluje edycję nieobecności.
        /// </summary>
        [RelayCommand]
        private static async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        /// <summary>
        /// Aktualizuje typ nieobecności.
        /// </summary>
        /// <param name="index">Indeks wybranego typu nieobecności.</param>
        [RelayCommand]
        private void UpdateAbsenceType(int index)
        {
            if (this.absence == null)
            {
                return;
            }

            this.absence.Type = (AbsenceType)index;

            // Set default values based on type
            switch (this.absence.Type)
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

        /// <summary>
        /// Usuwa nieobecność.
        /// </summary>
        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (!this.IsExistingAbsence || this.absence == null)
            {
                return;
            }

            var window = Application.Current?.Windows[0];
            var page = window?.Page;

            if (page == null)
            {
                return;
            }

            bool confirm = await page.DisplayAlert(
                "Potwierdzenie",
                "Czy na pewno chcesz usunąć tę nieobecność?",
                "Tak",
                "Nie");

            if (confirm)
            {
                try
                {
                    await this.databaseService.DeleteAsync(this.absence);
                    await Shell.Current.Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Error deleting absence");
                    await page.DisplayAlert(
                        "Błąd",
                        $"Nie udało się usunąć nieobecności: {ex.Message}",
                        "OK");
                }
            }
        }

        /// <summary>
        /// Zapisuje nieobecność.
        /// </summary>
        [RelayCommand]
        private async Task SaveAsync()
        {
            var window = Application.Current?.Windows[0];
            var page = window?.Page;

            if (page == null)
            {
                return;
            }

            // Validation
            if (this.StartDate > this.EndDate)
            {
                await page.DisplayAlert(
                    "Błąd",
                    "Data zakończenia musi być późniejsza lub równa dacie rozpoczęcia.",
                    "OK");
                return;
            }

            if (!int.TryParse(this.Year, out int yearValue))
            {
                await page.DisplayAlert(
                    "Błąd",
                    "Wprowadź poprawny rok.",
                    "OK");
                return;
            }

            // Upewnij się, że zawsze tworzysz NOWY obiekt Absence, nigdy nie modyfikuj istniejącego
            var newAbsence = new Absence
            {
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                DurationDays = int.Parse(this.DurationDays),
                Description = this.Description,
                AffectsSpecializationLength = this.AffectsSpecializationLength,
                DocumentReference = this.DocumentReference,
                Year = yearValue,
                IsApproved = this.IsApproved,
                Type = (AbsenceType)this.AbsenceTypeSelectedIndex,
            };

            this.onSaveCallback?.Invoke(newAbsence);

            await Shell.Current.Navigation.PopAsync();
        }
    }
}