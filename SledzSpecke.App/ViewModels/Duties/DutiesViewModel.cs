using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;
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
        }

        [ObservableProperty]
        private ObservableCollection<DutyViewModel> duties;

        [ObservableProperty]
        private DutyStatistics statistics;

        [ObservableProperty]
        private DateTime fromDate = DateTime.Today.AddMonths(-1);

        [ObservableProperty]
        private DateTime toDate = DateTime.Today.AddMonths(1);

        [ObservableProperty]
        private decimal totalHours;

        [ObservableProperty]
        private decimal monthlyHours;

        [ObservableProperty]
        private decimal requiredTotalHours;

        [ObservableProperty]
        private decimal requiredMonthlyHours;

        [ObservableProperty]
        private double progress;

        [ObservableProperty]
        private double monthlyProgress;

        [ObservableProperty]
        private List<DutyRequirements.DutySpecification> currentYearRequirements;

        [ObservableProperty]
        private string dutyRequirementsText;

        private int _currentSpecializationId;
        private int _currentSpecializationYear;

        // Display properties for binding
        public string TotalHoursDisplay => $"{TotalHours:F1} / {RequiredTotalHours:F1}h";
        public string MonthlyHoursDisplay => $"{MonthlyHours:F1} / {RequiredMonthlyHours:F1}h";

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var currentSpecialization = await _specializationService.GetCurrentSpecializationAsync();
                if (currentSpecialization != null)
                {
                    _currentSpecializationId = currentSpecialization.Id;
                    var user = await _userService.GetCurrentUserAsync();
                    if (user?.SpecializationStartDate != null)
                    {
                        var yearsInProgram = (DateTime.Today - user.SpecializationStartDate).Days / 365;
                        _currentSpecializationYear = Math.Max(1, Math.Min(6, yearsInProgram + 1));
                    }
                    else
                    {
                        _currentSpecializationYear = 1;
                    }

                    CurrentYearRequirements = _requirementsProvider.GetDutyRequirementsBySpecialization(_currentSpecializationId)
                        .Where(r => r.Year == _currentSpecializationYear)
                        .ToList();

                    if (CurrentYearRequirements.Any())
                    {
                        var requirement = CurrentYearRequirements.First();

                        // Update required hours based on requirements
                        RequiredMonthlyHours = requirement.MinimumHoursPerMonth;
                        // Calculate total required hours for the year (12 months)
                        RequiredTotalHours = requirement.MinimumHoursPerMonth * 12;

                        DutyRequirementsText = $"Wymagania na rok {_currentSpecializationYear}:\n" +
                                               $"Min. {requirement.MinimumHoursPerMonth} godz./m-c\n" +
                                               $"Min. {requirement.MinimumDutiesPerMonth} dyżurów/m-c";

                        if (requirement.RequiresSupervision)
                        {
                            DutyRequirementsText += "\nWymagany nadzór";
                        }
                    }
                    else
                    {
                        // Default values if no requirements found
                        RequiredMonthlyHours = 40; // Default value from DutyRequirements
                        RequiredTotalHours = 480; // 40 * 12 months
                        DutyRequirementsText = "Standardowe wymagania:\nMin. 40 godz./m-c";
                    }
                }
                else
                {
                    RequiredMonthlyHours = 40;
                    RequiredTotalHours = 480;
                    DutyRequirementsText = "Brak danych o specjalizacji";
                }

                var userDuties = await _dutyService.GetUserDutiesAsync(FromDate);
                Statistics = await _dutyService.GetDutyStatisticsAsync();

                TotalHours = Statistics.TotalHours;

                // Calculate current month hours
                var currentMonthDuties = userDuties
                    .Where(d => d.StartTime.Month == DateTime.Today.Month && d.StartTime.Year == DateTime.Today.Year)
                    .ToList();
                MonthlyHours = currentMonthDuties.Sum(d => (decimal)(d.EndTime - d.StartTime).TotalHours);

                // Calculate progress relative to requirements
                Progress = Math.Min(1.0, (double)(TotalHours / RequiredTotalHours));
                MonthlyProgress = Math.Min(1.0, (double)(MonthlyHours / RequiredMonthlyHours));

                UpdateDutiesList(userDuties);

                await ApplyFiltersAsync();

                // Force UI update for calculated properties
                OnPropertyChanged(nameof(TotalHoursDisplay));
                OnPropertyChanged(nameof(MonthlyHoursDisplay));
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
                    StartTime = duty.StartTime,
                    EndTime = duty.EndTime,
                    Notes = duty.Notes
                });
            }
        }

        private async Task ApplyFiltersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var userDuties = await _dutyService.GetUserDutiesAsync(FromDate);

                userDuties = userDuties.Where(d =>
                    d.StartTime.Date >= FromDate.Date &&
                    d.StartTime.Date <= ToDate.Date).ToList();

                UpdateDutiesList(userDuties);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Błąd",
                    $"Nie udało się zafiltrować dyżurów: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
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
            var navigationParameter = new Dictionary<string, object>
            {
                { "ReportType", "Statystyki dyżurów" }
            };

            await Shell.Current.GoToAsync("reports", navigationParameter);
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
