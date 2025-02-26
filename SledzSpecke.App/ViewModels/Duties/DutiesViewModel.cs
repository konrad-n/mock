using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Monitoring;
using SledzSpecke.Core.Models.Requirements;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.ViewModels.Duties
{
    public partial class DutiesViewModel : BaseViewModel
    {
        private readonly IDutyService _dutyService;
        private readonly ISpecializationService _specializationService;
        private readonly ISpecializationRequirementsProvider _requirementsProvider;
        private readonly IUserService _userService;

        public DutiesViewModel(
            IDutyService dutyService,
            ISpecializationService specializationService,
            ISpecializationRequirementsProvider requirementsProvider,
            IUserService userService)
        {
            _dutyService = dutyService;
            _specializationService = specializationService;
            _requirementsProvider = requirementsProvider;
            _userService = userService;

            Title = "Dyżury";

            Duties = new ObservableCollection<DutyViewModel>();
            DutyTypes = new ObservableCollection<string> { "Wszystkie", "Regular", "Emergency", "Weekend", "Holiday", "Supervised" };
            SelectedDutyType = "Wszystkie";
        }

        [ObservableProperty]
        private ObservableCollection<DutyViewModel> duties;

        [ObservableProperty]
        private DutyStatistics statistics;

        [ObservableProperty]
        private ObservableCollection<string> dutyTypes;

        [ObservableProperty]
        private string selectedDutyType;

        [ObservableProperty]
        private DateTime fromDate = DateTime.Today.AddMonths(-1);

        [ObservableProperty]
        private DateTime toDate = DateTime.Today.AddMonths(1);

        [ObservableProperty]
        private decimal totalHours;

        [ObservableProperty]
        private decimal monthlyHours;

        [ObservableProperty]
        private decimal remainingHours;

        [ObservableProperty]
        private double progress;

        [ObservableProperty]
        private List<DutyRequirements.DutySpecification> currentYearRequirements;

        [ObservableProperty]
        private string dutyRequirementsText;

        private int _currentSpecializationId;
        private int _currentSpecializationYear;

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Pobierz bieżącą specjalizację
                var currentSpecialization = await _specializationService.GetCurrentSpecializationAsync();
                if (currentSpecialization != null)
                {
                    _currentSpecializationId = currentSpecialization.Id;

                    // Oblicz obecny rok specjalizacji
                    var user = await _userService.GetCurrentUserAsync();
                    if (user?.SpecializationStartDate != null)
                    {
                        var yearsInProgram = (DateTime.Today - user.SpecializationStartDate).Days / 365;
                        _currentSpecializationYear = Math.Max(1, Math.Min(6, yearsInProgram + 1));
                    }
                    else
                    {
                        _currentSpecializationYear = 1; // domyślnie pierwszy rok
                    }

                    // Pobierz wymagania dla bieżącego roku
                    CurrentYearRequirements = _requirementsProvider.GetDutyRequirementsBySpecialization(_currentSpecializationId)
                        .Where(r => r.Year == _currentSpecializationYear)
                        .ToList();

                    if (CurrentYearRequirements.Any())
                    {
                        var requirement = CurrentYearRequirements.First();
                        DutyRequirementsText = $"Wymagania na rok {_currentSpecializationYear}:\n" +
                                               $"Min. {requirement.MinimumHoursPerMonth} godz./m-c\n" +
                                               $"Min. {requirement.MinimumDutiesPerMonth} dyżurów/m-c";

                        if (requirement.RequiresSupervision)
                        {
                            DutyRequirementsText += "\nWymagany nadzór";
                        }
                    }
                }
                else
                {
                    DutyRequirementsText = "Brak danych o specjalizacji";
                }

                // Pobierz dyżury
                var userDuties = await _dutyService.GetUserDutiesAsync(FromDate);

                // Pobierz statystyki
                Statistics = await _dutyService.GetDutyStatisticsAsync();

                TotalHours = Statistics.TotalHours;
                MonthlyHours = Statistics.MonthlyHours;
                RemainingHours = Statistics.RemainingHours > 0 ? Statistics.RemainingHours : 1;

                // Oblicz postęp
                Progress = (double)(TotalHours / (TotalHours + RemainingHours));

                UpdateDutiesList(userDuties);

                await ApplyFiltersAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Błąd",
                    $"Nie udało się załadować dyżurów: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateDutiesList(List<Duty> userDuties)
        {
            Duties.Clear();
            foreach (var duty in userDuties)
            {
                Duties.Add(new DutyViewModel
                {
                    Id = duty.Id,
                    Location = duty.Location,
                    Date = duty.StartTime.Date,
                    Hours = (decimal)(duty.EndTime - duty.StartTime).TotalHours,
                    Type = duty.Type.ToString(),
                    StartTime = duty.StartTime,
                    EndTime = duty.EndTime,
                    Notes = duty.Notes
                });
            }
        }

        partial void OnSelectedDutyTypeChanged(string value)
        {
            _ = ApplyFiltersAsync();
        }

        private async Task ApplyFiltersAsync()
        {
            // Istniejąca implementacja...
        }

        [RelayCommand]
        private async Task AddDutyAsync()
        {
            await Shell.Current.GoToAsync("duty/add");
        }

        [RelayCommand]
        private async Task ViewDutyAsync(int id)
        {
            await Shell.Current.GoToAsync($"duty/edit?id={id}");
        }

        [RelayCommand]
        private async Task ExportDutiesAsync()
        {
            // Konwersja dyżurów do modelu monitorowania
            var monthlyDuties = new List<DutyMonitoring.Duty>();

            foreach (var dutyVm in Duties)
            {
                monthlyDuties.Add(new DutyMonitoring.Duty
                {
                    StartTime = dutyVm.StartTime,
                    EndTime = dutyVm.EndTime,
                    Type = dutyVm.Type,
                    Location = dutyVm.Location,
                    WasSupervised = dutyVm.Type.Contains("Supervised")
                });
            }

            // Weryfikacja zgodności z wymaganiami
            if (CurrentYearRequirements.Any())
            {
                var validator = new DutyMonitoring.DutyValidator();
                var (isCompliant, deficiencies) = validator.CheckMonthlyCompliance(
                    _currentSpecializationId, _currentSpecializationYear, monthlyDuties);

                var stats = validator.GenerateStatistics(monthlyDuties);
                var report = validator.GenerateReport(stats);

                if (!isCompliant)
                {
                    report += "\n\nDeficyty:\n" + string.Join("\n", deficiencies);
                }

                await Shell.Current.DisplayAlert("Raport dyżurowy", report, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Eksport dyżurów",
                    "Funkcja eksportu dyżurów zostanie zaimplementowana wkrótce.",
                    "OK");
            }
        }
    }

    public class DutyViewModel
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public string Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Notes { get; set; }
    }
}
