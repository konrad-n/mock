using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.Absences.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.Features.Absences.ViewModels
{
    public partial class AbsenceManagementViewModel : ViewModelBase
    {
        private readonly ISpecializationService _specializationService;
        private readonly ISpecializationDateCalculator _specializationDateCalculator;
        private readonly IDatabaseService _databaseService;

        [ObservableProperty]
        private Specialization _specialization;

        [ObservableProperty]
        private ObservableCollection<Absence> _allAbsences;

        [ObservableProperty]
        private ObservableCollection<Absence> _filteredAbsences;

        [ObservableProperty]
        private AbsenceType? _selectedAbsenceType;

        [ObservableProperty]
        private int? _selectedYear;

        [ObservableProperty]
        private string _plannedEndDateLabel;

        [ObservableProperty]
        private string _actualEndDateLabel;

        [ObservableProperty]
        private string _selfEducationDaysLabel;

        [ObservableProperty]
        private string _totalAbsenceDaysLabel;

        [ObservableProperty]
        private bool _isNoAbsencesVisible;

        [ObservableProperty]
        private ObservableCollection<int> _availableYears;

        public AbsenceManagementViewModel(
            ISpecializationService specializationService,
            ISpecializationDateCalculator specializationDateCalculator,
            IDatabaseService databaseService,
            ILogger<AbsenceManagementViewModel> logger) : base(logger)
        {
            _specializationService = specializationService;
            _specializationDateCalculator = specializationDateCalculator;
            _databaseService = databaseService;

            AllAbsences = new ObservableCollection<Absence>();
            FilteredAbsences = new ObservableCollection<Absence>();
            AvailableYears = new ObservableCollection<int>();

            Title = "Nieobecności i urlopy";
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading absences data");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Load specialization data
                Specialization = await _specializationService.GetSpecializationAsync();

                // Calculate dates
                DateTime plannedEndDate = Specialization.StartDate.AddDays(Specialization.BaseDurationWeeks * 7);
                DateTime actualEndDate = await _specializationDateCalculator.CalculateExpectedEndDateAsync(Specialization.Id);

                // Update UI with dates
                PlannedEndDateLabel = plannedEndDate.ToString("dd.MM.yyyy");
                ActualEndDateLabel = actualEndDate.ToString("dd.MM.yyyy");

                // Calculate self-education days
                int currentYear = DateTime.Now.Year;
                int remainingSelfEducationDays = await _specializationDateCalculator.GetRemainingEducationDaysForYearAsync(Specialization.Id, currentYear);
                int usedSelfEducationDays = Specialization.SelfEducationDaysPerYear - remainingSelfEducationDays;

                SelfEducationDaysLabel = $"{usedSelfEducationDays}/{Specialization.SelfEducationDaysPerYear}";

                // Load absences
                var absences = await _databaseService.QueryAsync<Absence>(
                    "SELECT * FROM Absences WHERE SpecializationId = ? ORDER BY StartDate DESC",
                    Specialization.Id);

                AllAbsences = new ObservableCollection<Absence>(absences);

                // Update total absence days
                int totalAbsenceDays = AllAbsences.Sum(a => a.DurationDays);
                TotalAbsenceDaysLabel = totalAbsenceDays.ToString();

                // Setup filter year picker
                SetupYearFilter();

                // Apply filters and display absences
                ApplyFiltersAndDisplayAbsences();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading absence data");
                throw;
            }
        }

        private void SetupYearFilter()
        {
            // Get unique years from absences
            var years = AllAbsences
                .Select(a => a.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            AvailableYears.Clear();

            // All years
            AvailableYears.Add(0); // 0 represents "All years"

            // Add individual years
            foreach (var year in years)
            {
                AvailableYears.Add(year);
            }
        }

        public void ApplyFiltersAndDisplayAbsences()
        {
            // Filter absences
            var filtered = AllAbsences.AsEnumerable();

            // Filter by type if selected
            if (SelectedAbsenceType.HasValue)
            {
                filtered = filtered.Where(a => a.Type == SelectedAbsenceType.Value);
            }

            // Filter by year if selected
            if (SelectedYear.HasValue && SelectedYear.Value > 0)
            {
                filtered = filtered.Where(a => a.Year == SelectedYear.Value);
            }

            // Update filtered absences
            FilteredAbsences = new ObservableCollection<Absence>(filtered);

            // Show/hide no absences message
            IsNoAbsencesVisible = FilteredAbsences.Count == 0;
        }

        [RelayCommand]
        private async Task AddAbsenceAsync()
        {
            await Shell.Current.Navigation.PushAsync(new AbsenceDetailsPage(_databaseService, null, OnAbsenceAdded));
        }

        [RelayCommand]
        private async Task EditAbsenceAsync(int absenceId)
        {
            var absence = AllAbsences.FirstOrDefault(a => a.Id == absenceId);
            if (absence != null)
            {
                await Shell.Current.Navigation.PushAsync(new AbsenceDetailsPage(_databaseService, absence, OnAbsenceUpdated));
            }
        }

        [RelayCommand]
        private void FilterByAbsenceType(int typeIndex)
        {
            // Map picker index to absence type
            SelectedAbsenceType = typeIndex switch
            {
                1 => AbsenceType.SickLeave,
                2 => AbsenceType.VacationLeave,
                3 => AbsenceType.SelfEducationLeave,
                4 => null, // Special handling for maternity/parental leaves
                5 => null, // Special handling for other leave types
                _ => null  // No filter (all types)
            };

            ApplyFiltersAndDisplayAbsences();
        }

        [RelayCommand]
        private void FilterByYear(int yearIndex)
        {
            // Year index 0 means "All years"
            SelectedYear = yearIndex > 0 ? AvailableYears[yearIndex] : null;
            ApplyFiltersAndDisplayAbsences();
        }

        private async void OnAbsenceAdded(Absence absence)
        {
            try
            {
                // Save absence
                absence.SpecializationId = Specialization.Id;

                // Ustaw Id na 0 (lub null), aby wymusić wstawienie nowego rekordu
                absence.Id = 0;

                // Użyj metody specyficznej do wstawiania (insert) nowego rekordu
                // zamiast metody SaveAsync, która może używać ID do decydowania czy wykonać update
                await _databaseService.InsertAsync(absence);

                // Dla pewności, dodaj log który pokazuje nowe ID
                _logger.LogInformation($"Added new absence with ID: {absence.Id}");

                // Reload data
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding absence");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił problem podczas dodawania nieobecności: {ex.Message}", "OK");
            }
        }

        private async void OnAbsenceUpdated(Absence absence)
        {
            // Update absence
            absence.SpecializationId = Specialization.Id;
            await _databaseService.SaveAsync(absence);

            // Reload data
            await LoadDataAsync();
        }

        public string GetAbsenceTypeText(AbsenceType type)
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
                _ => "Nieobecność"
            };
        }

        public Color GetAbsenceCardColor(AbsenceType type)
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

        public string GetAbsenceIconText(AbsenceType type)
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
    }
}