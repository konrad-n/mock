// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsenceManagementViewModel.cs" company="SledzSpecke">
//   Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   ViewModel zarządzania nieobecnościami.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.Absences.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Features.Absences.ViewModels
{
    /// <summary>
    /// ViewModel zarządzania nieobecnościami.
    /// </summary>
    public partial class AbsenceManagementViewModel : ViewModelBase
    {
        private readonly ISpecializationService specializationService;
        private readonly ISpecializationDateCalculator specializationDateCalculator;
        private readonly IDatabaseService databaseService;

        // Używamy partial properties zamiast [ObservableProperty] dla zgodności z AOT
        private Specialization specialization = null!;
        private ObservableCollection<Absence> allAbsences = new ();
        private ObservableCollection<Absence> filteredAbsences = new ();
        private AbsenceType? selectedAbsenceType;
        private int? selectedYear;
        private string plannedEndDateLabel = string.Empty;
        private string actualEndDateLabel = string.Empty;
        private string selfEducationDaysLabel = string.Empty;
        private string totalAbsenceDaysLabel = string.Empty;
        private bool isNoAbsencesVisible;
        private ObservableCollection<int> availableYears = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="AbsenceManagementViewModel"/> class.
        /// </summary>
        /// <param name="specializationService">Serwis specjalizacji.</param>
        /// <param name="specializationDateCalculator">Kalkulator dat specjalizacji.</param>
        /// <param name="databaseService">Serwis bazy danych.</param>
        /// <param name="logger">Logger.</param>
        public AbsenceManagementViewModel(
            ISpecializationService specializationService,
            ISpecializationDateCalculator specializationDateCalculator,
            IDatabaseService databaseService,
            ILogger<AbsenceManagementViewModel> logger)
            : base(logger)
        {
            this.specializationService = specializationService;
            this.specializationDateCalculator = specializationDateCalculator;
            this.databaseService = databaseService;

            this.Title = "Nieobecności i urlopy";
        }

        /// <summary>
        /// Gets or sets specjalizację.
        /// </summary>
        public Specialization Specialization
        {
            get => this.specialization;
            set => this.SetProperty(ref this.specialization, value);
        }

        /// <summary>
        /// Gets or sets wszystkie nieobecności.
        /// </summary>
        public ObservableCollection<Absence> AllAbsences
        {
            get => this.allAbsences;
            set => this.SetProperty(ref this.allAbsences, value);
        }

        /// <summary>
        /// Gets or sets przefiltrowane nieobecności.
        /// </summary>
        public ObservableCollection<Absence> FilteredAbsences
        {
            get => this.filteredAbsences;
            set => this.SetProperty(ref this.filteredAbsences, value);
        }

        /// <summary>
        /// Gets or sets wybrany typ nieobecności.
        /// </summary>
        public AbsenceType? SelectedAbsenceType
        {
            get => this.selectedAbsenceType;
            set => this.SetProperty(ref this.selectedAbsenceType, value);
        }

        /// <summary>
        /// Gets or sets wybrany rok.
        /// </summary>
        public int? SelectedYear
        {
            get => this.selectedYear;
            set => this.SetProperty(ref this.selectedYear, value);
        }

        /// <summary>
        /// Gets or sets etykietę planowanej daty zakończenia specjalizacji.
        /// </summary>
        public string PlannedEndDateLabel
        {
            get => this.plannedEndDateLabel;
            set => this.SetProperty(ref this.plannedEndDateLabel, value);
        }

        /// <summary>
        /// Gets or sets etykietę faktycznej daty zakończenia specjalizacji.
        /// </summary>
        public string ActualEndDateLabel
        {
            get => this.actualEndDateLabel;
            set => this.SetProperty(ref this.actualEndDateLabel, value);
        }

        /// <summary>
        /// Gets or sets etykietę dni samokształcenia.
        /// </summary>
        public string SelfEducationDaysLabel
        {
            get => this.selfEducationDaysLabel;
            set => this.SetProperty(ref this.selfEducationDaysLabel, value);
        }

        /// <summary>
        /// Gets or sets etykietę sumy dni nieobecności.
        /// </summary>
        public string TotalAbsenceDaysLabel
        {
            get => this.totalAbsenceDaysLabel;
            set => this.SetProperty(ref this.totalAbsenceDaysLabel, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether komunikat o braku nieobecności jest widoczny.
        /// </summary>
        public bool IsNoAbsencesVisible
        {
            get => this.isNoAbsencesVisible;
            set => this.SetProperty(ref this.isNoAbsencesVisible, value);
        }

        /// <summary>
        /// Gets or sets dostępne lata.
        /// </summary>
        public ObservableCollection<int> AvailableYears
        {
            get => this.availableYears;
            set => this.SetProperty(ref this.availableYears, value);
        }

        /// <summary>
        /// Zwraca opis typu nieobecności.
        /// </summary>
        /// <param name="type">Typ nieobecności.</param>
        /// <returns>Opis typu nieobecności.</returns>
        public static string GetAbsenceTypeText(AbsenceType type)
        {
            return type switch
            {
                AbsenceType.SickLeave => "Zwolnienie lekarskie (L4)",
                AbsenceType.VacationLeave => "Urlop wypoczynkowy",
                AbsenceType.SelfEducationLeave => "Urlop szkoleniowy (samokształcenie)",
                AbsenceType.MaternityLeave => "Urlop macierzyński",
                AbsenceType.ParentalLeave => "Urlop rodzicielski",
                AbsenceType.SpecialLeave => "Urlop okolicznościowy",
                AbsenceType.UnpaidLeave => "Urlop bezpłatny",
                AbsenceType.Other => "Inna nieobecność",
                _ => "Nieobecność",
            };
        }

        /// <summary>
        /// Zwraca kolor karty dla typu nieobecności.
        /// </summary>
        /// <param name="type">Typ nieobecności.</param>
        /// <returns>Kolor karty.</returns>
        public static Color GetAbsenceCardColor(AbsenceType type)
        {
            return type switch
            {
                AbsenceType.SickLeave => Color.FromArgb("#FFE0E0"),
                AbsenceType.VacationLeave => Color.FromArgb("#E0F7FA"),
                AbsenceType.SelfEducationLeave => Color.FromArgb("#E8F5E9"),
                AbsenceType.MaternityLeave => Color.FromArgb("#FFF8E1"),
                AbsenceType.ParentalLeave => Color.FromArgb("#FFF8E1"),
                _ => Color.FromArgb("#F5F5F5")
            };
        }

        /// <summary>
        /// Zwraca ikonę tekstową dla typu nieobecności.
        /// </summary>
        /// <param name="type">Typ nieobecności.</param>
        /// <returns>Ikona tekstowa (emoji).</returns>
        public static string GetAbsenceIconText(AbsenceType type)
        {
            return type switch
            {
                AbsenceType.SickLeave => "🤒",
                AbsenceType.VacationLeave => "🏖️",
                AbsenceType.SelfEducationLeave => "📚",
                AbsenceType.MaternityLeave => "👶",
                AbsenceType.ParentalLeave => "👶",
                _ => "📅"
            };
        }

        /// <summary>
        /// Inicjalizuje ViewModel.
        /// </summary>
        /// <returns>Task reprezentujący operację asynchroniczną.</returns>
        public override async Task InitializeAsync()
        {
            try
            {
                this.IsBusy = true;
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading absences data");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Ładuje dane nieobecności.
        /// </summary>
        /// <returns>Task reprezentujący operację asynchroniczną.</returns>
        public async Task LoadDataAsync()
        {
            try
            {
                // Load specialization data
                this.Specialization = await this.specializationService.GetSpecializationAsync();

                // Calculate dates
                DateTime plannedEndDate = this.Specialization.StartDate.AddDays(this.Specialization.BaseDurationWeeks * 7);
                DateTime actualEndDate = await this.specializationDateCalculator.CalculateExpectedEndDateAsync(this.Specialization.Id);

                // Update UI with dates
                this.PlannedEndDateLabel = plannedEndDate.ToString("dd.MM.yyyy");
                this.ActualEndDateLabel = actualEndDate.ToString("dd.MM.yyyy");

                // Calculate self-education days
                int currentYear = DateTime.Now.Year;
                int remainingSelfEducationDays = await this.specializationDateCalculator.GetRemainingEducationDaysForYearAsync(this.Specialization.Id, currentYear);
                int usedSelfEducationDays = this.Specialization.SelfEducationDaysPerYear - remainingSelfEducationDays;

                this.SelfEducationDaysLabel = $"{usedSelfEducationDays}/{this.Specialization.SelfEducationDaysPerYear}";

                // Load absences
                var absences = await this.databaseService.QueryAsync<Absence>(
                    "SELECT * FROM Absences WHERE SpecializationId = ? ORDER BY StartDate DESC",
                    this.Specialization.Id);

                this.AllAbsences = new ObservableCollection<Absence>(absences);

                // Update total absence days
                int totalAbsenceDays = this.AllAbsences.Sum(a => a.DurationDays);
                this.TotalAbsenceDaysLabel = totalAbsenceDays.ToString();

                // Setup filter year picker
                this.SetupYearFilter();

                // Apply filters and display absences
                this.ApplyFiltersAndDisplayAbsences();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading absence data"); // Nie rzucaj wyjątku ponownie, tylko zaloguj błąd
            }
        }

        /// <summary>
        /// Stosuje filtry i wyświetla nieobecności.
        /// </summary>
        public void ApplyFiltersAndDisplayAbsences()
        {
            // Filter absences
            var filtered = this.AllAbsences.AsEnumerable();

            // Filter by type if selected
            if (this.SelectedAbsenceType.HasValue)
            {
                filtered = filtered.Where(a => a.Type == this.SelectedAbsenceType.Value);
            }

            // Filter by year if selected
            if (this.SelectedYear.HasValue && this.SelectedYear.Value > 0)
            {
                filtered = filtered.Where(a => a.Year == this.SelectedYear.Value);
            }

            // Update filtered absences
            this.FilteredAbsences = new ObservableCollection<Absence>(filtered);

            // Show/hide no absences message
            this.IsNoAbsencesVisible = this.FilteredAbsences.Count == 0;
        }

        /// <summary>
        /// Konfiguruje filtr roku.
        /// </summary>
        private void SetupYearFilter()
        {
            // Get unique years from absences
            var years = this.AllAbsences
                .Select(a => a.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            this.AvailableYears.Clear();

            // All years
            this.AvailableYears.Add(0); // 0 represents "All years"

            // Add individual years
            foreach (var year in years)
            {
                this.AvailableYears.Add(year);
            }
        }

        /// <summary>
        /// Dodaje nową nieobecność.
        /// </summary>
        [RelayCommand]
        private async Task AddAbsenceAsync()
        {
            await Shell.Current.Navigation.PushAsync(new AbsenceDetailsPage(absence: null, this.OnAbsenceAdded));
        }

        /// <summary>
        /// Edytuje nieobecność.
        /// </summary>
        /// <param name="absenceId">Identyfikator nieobecności.</param>
        [RelayCommand]
        private async Task EditAbsenceAsync(int absenceId)
        {
            var absence = this.AllAbsences.FirstOrDefault(a => a.Id == absenceId);
            if (absence != null)
            {
                await Shell.Current.Navigation.PushAsync(new AbsenceDetailsPage(absence, this.OnAbsenceUpdated));
            }
        }

        /// <summary>
        /// Filtruje nieobecności według typu.
        /// </summary>
        /// <param name="typeIndex">Indeks typu nieobecności.</param>
        [RelayCommand]
        private void FilterByAbsenceType(int typeIndex)
        {
            // Map picker index to absence type
            this.SelectedAbsenceType = typeIndex switch
            {
                1 => AbsenceType.SickLeave,
                2 => AbsenceType.VacationLeave,
                3 => AbsenceType.SelfEducationLeave,
                4 => null, // Special handling for maternity/parental leaves
                5 => null, // Special handling for other leave types
                _ => null, // No filter (all types)
            };

            this.ApplyFiltersAndDisplayAbsences();
        }

        /// <summary>
        /// Filtruje nieobecności według roku.
        /// </summary>
        /// <param name="yearIndex">Indeks roku.</param>
        [RelayCommand]
        private void FilterByYear(int yearIndex)
        {
            // Year index 0 means "All years"
            this.SelectedYear = yearIndex > 0 ? this.AvailableYears[yearIndex] : null;
            this.ApplyFiltersAndDisplayAbsences();
        }

        /// <summary>
        /// Obsługuje dodanie nieobecności.
        /// </summary>
        /// <param name="absence">Dodana nieobecność.</param>
        private async void OnAbsenceAdded(Absence absence)
        {
            try
            {
                // Save absence
                absence.SpecializationId = this.Specialization.Id;

                // Ustaw Id na 0 (lub null), aby wymusić wstawienie nowego rekordu
                absence.Id = 0;

                // Użyj metody specyficznej do wstawiania (insert) nowego rekordu
                // zamiast metody SaveAsync, która może używać ID do decydowania czy wykonać update
                await this.databaseService.InsertAsync(absence);

                // Dla pewności, dodaj log który pokazuje nowe ID
                this.logger.LogInformation("Added new absence with ID: {AbsenceId}", absence.Id);

                // Reload data
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error adding absence");
                var window = Application.Current?.Windows[0];
                var page = window?.Page;

                if (page != null)
                {
                    await page.DisplayAlert("Błąd", $"Wystąpił problem podczas dodawania nieobecności: {ex.Message}", "OK");
                }
            }
        }

        /// <summary>
        /// Obsługuje aktualizację nieobecności.
        /// </summary>
        /// <param name="absence">Zaktualizowana nieobecność.</param>
        private async void OnAbsenceUpdated(Absence absence)
        {
            // Update absence
            absence.SpecializationId = this.Specialization.Id;
            await this.databaseService.SaveAsync(absence);

            // Reload data
            await this.LoadDataAsync();
        }
    }
}