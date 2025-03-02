using System.Collections.ObjectModel;
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
    public partial class AbsenceManagementViewModel : ViewModelBase
    {
        private readonly ISpecializationService specializationService;
        private readonly ISpecializationDateCalculator specializationDateCalculator;
        private readonly IDatabaseService databaseService;
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

        public Specialization Specialization
        {
            get => this.specialization;
            set => this.SetProperty(ref this.specialization, value);
        }

        public ObservableCollection<Absence> AllAbsences
        {
            get => this.allAbsences;
            set => this.SetProperty(ref this.allAbsences, value);
        }

        public ObservableCollection<Absence> FilteredAbsences
        {
            get => this.filteredAbsences;
            set => this.SetProperty(ref this.filteredAbsences, value);
        }

        public AbsenceType? SelectedAbsenceType
        {
            get => this.selectedAbsenceType;
            set => this.SetProperty(ref this.selectedAbsenceType, value);
        }

        public int? SelectedYear
        {
            get => this.selectedYear;
            set => this.SetProperty(ref this.selectedYear, value);
        }

        public string PlannedEndDateLabel
        {
            get => this.plannedEndDateLabel;
            set => this.SetProperty(ref this.plannedEndDateLabel, value);
        }

        public string ActualEndDateLabel
        {
            get => this.actualEndDateLabel;
            set => this.SetProperty(ref this.actualEndDateLabel, value);
        }

        public string SelfEducationDaysLabel
        {
            get => this.selfEducationDaysLabel;
            set => this.SetProperty(ref this.selfEducationDaysLabel, value);
        }

        public string TotalAbsenceDaysLabel
        {
            get => this.totalAbsenceDaysLabel;
            set => this.SetProperty(ref this.totalAbsenceDaysLabel, value);

        public bool IsNoAbsencesVisible
        {
            get => this.isNoAbsencesVisible;
            set => this.SetProperty(ref this.isNoAbsencesVisible, value);
        }

        public ObservableCollection<int> AvailableYears
        {
            get => this.availableYears;
            set => this.SetProperty(ref this.availableYears, value);
        }

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

        public async Task LoadDataAsync()
        {
            try
            {
                this.Specialization = await this.specializationService.GetSpecializationAsync();
                DateTime plannedEndDate = this.Specialization.StartDate.AddDays(this.Specialization.BaseDurationWeeks * 7);
                DateTime actualEndDate = await this.specializationDateCalculator.CalculateExpectedEndDateAsync(this.Specialization.Id);
                this.PlannedEndDateLabel = plannedEndDate.ToString("dd.MM.yyyy");
                this.ActualEndDateLabel = actualEndDate.ToString("dd.MM.yyyy");
                int currentYear = DateTime.Now.Year;
                int remainingSelfEducationDays = await this.specializationDateCalculator.GetRemainingEducationDaysForYearAsync(this.Specialization.Id, currentYear);
                int usedSelfEducationDays = this.Specialization.SelfEducationDaysPerYear - remainingSelfEducationDays;
                this.SelfEducationDaysLabel = $"{usedSelfEducationDays}/{this.Specialization.SelfEducationDaysPerYear}";
                var absences = await this.databaseService.QueryAsync<Absence>(
                    "SELECT * FROM Absences WHERE SpecializationId = ? ORDER BY StartDate DESC",
                    this.Specialization.Id);
                this.AllAbsences = new ObservableCollection<Absence>(absences);
                int totalAbsenceDays = this.AllAbsences.Sum(a => a.DurationDays);
                this.TotalAbsenceDaysLabel = totalAbsenceDays.ToString();
                this.SetupYearFilter();
                this.ApplyFiltersAndDisplayAbsences();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading absence data")
            }
        }

        public void ApplyFiltersAndDisplayAbsences()
        {
            var filtered = this.AllAbsences.AsEnumerable();
            if (this.SelectedAbsenceType.HasValue)
            {
                filtered = filtered.Where(a => a.Type == this.SelectedAbsenceType.Value);
            }

            if (this.SelectedYear.HasValue && this.SelectedYear.Value > 0)
            {
                filtered = filtered.Where(a => a.Year == this.SelectedYear.Value);
            }

            this.FilteredAbsences = new ObservableCollection<Absence>(filtered);
            this.IsNoAbsencesVisible = this.FilteredAbsences.Count == 0;
        }

        private void SetupYearFilter()
        {
            var years = this.AllAbsences
                .Select(a => a.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();
            this.AvailableYears.Clear();
            this.AvailableYears.Add(0);
            foreach (var year in years)
            {
                this.AvailableYears.Add(year);
            }
        }

        [RelayCommand]
        private async Task AddAbsenceAsync()
        {
            await Shell.Current.Navigation.PushAsync(new AbsenceDetailsPage(absence: null, this.OnAbsenceAdded));
        }

        [RelayCommand]
        private async Task EditAbsenceAsync(int absenceId)
        {
            var absence = this.AllAbsences.FirstOrDefault(a => a.Id == absenceId);
            if (absence != null)
            {
                await Shell.Current.Navigation.PushAsync(new AbsenceDetailsPage(absence, this.OnAbsenceUpdated));
            }
        }

        [RelayCommand]
        private void FilterByAbsenceType(int typeIndex)
        {
            this.SelectedAbsenceType = typeIndex switch
            {
                1 => AbsenceType.SickLeave,
                2 => AbsenceType.VacationLeave,
                3 => AbsenceType.SelfEducationLeave,
                4 => null,
                5 => null,
                _ => null,
            };

            this.ApplyFiltersAndDisplayAbsences();
        }

        [RelayCommand]
        private void FilterByYear(int yearIndex)
        {
            this.SelectedYear = yearIndex > 0 ? this.AvailableYears[yearIndex] : null;
            this.ApplyFiltersAndDisplayAbsences();
        }

        private async void OnAbsenceAdded(Absence absence)
        {
            try
            {
                absence.SpecializationId = this.Specialization.Id;
                absence.Id = 0;
                await this.databaseService.InsertAsync(absence);

                this.logger.LogInformation("Added new absence with ID: {AbsenceId}", absence.Id);

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

        private async void OnAbsenceUpdated(Absence absence)
        {
            absence.SpecializationId = this.Specialization.Id;
            await this.databaseService.SaveAsync(absence);

            await this.LoadDataAsync();
        }
    }
}