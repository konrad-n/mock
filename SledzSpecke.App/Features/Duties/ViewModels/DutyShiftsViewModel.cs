using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.Duties.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.Features.Duties.ViewModels
{
    public partial class DutyShiftsViewModel : ViewModelBase
    {
        private readonly IDutyShiftService _dutyShiftService;
        private readonly ISpecializationService _specializationService;
        private Specialization _specialization;
        private double _totalRequiredHours;

        [ObservableProperty]
        private ObservableCollection<DutyShift> _dutyShifts;

        [ObservableProperty]
        private string _totalHoursLabel;

        [ObservableProperty]
        private string _weeklyHoursLabel;

        [ObservableProperty]
        private bool _isNoDutyShiftsVisible;

        [ObservableProperty]
        private ObservableCollection<GroupedDutyShifts> _groupedDutyShifts;

        public DutyShiftsViewModel(
            IDutyShiftService dutyShiftService,
            ISpecializationService specializationService,
            ILogger<DutyShiftsViewModel> logger) : base(logger)
        {
            _dutyShiftService = dutyShiftService;
            _specializationService = specializationService;
            DutyShifts = new ObservableCollection<DutyShift>();
            GroupedDutyShifts = new ObservableCollection<GroupedDutyShifts>();
            Title = "Dyżury";
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
                _logger.LogError(ex, "Error loading duty shifts data");
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
                // Get all duty shifts from the database
                var dutyShifts = await _dutyShiftService.GetAllDutyShiftsAsync();
                DutyShifts = new ObservableCollection<DutyShift>(dutyShifts);

                // Get specialization for required hours
                _specialization = await _specializationService.GetSpecializationAsync();
                _totalRequiredHours = _specialization.RequiredDutyHoursPerWeek * (_specialization.BaseDurationWeeks / 52.0) * 52;

                // Get weekly average
                double weeklyAverage = CalculateWeeklyAverage();

                // Update UI
                UpdateTotalHours();
                GroupAndDisplayDutyShifts();

                // Update weekly hours label
                WeeklyHoursLabel = $"{weeklyAverage:F1}h";

                // Show/hide no duty shifts message
                IsNoDutyShiftsVisible = DutyShifts.Count == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading duty shifts");
                throw;
            }
        }

        private double CalculateWeeklyAverage()
        {
            try
            {
                if (DutyShifts == null || DutyShifts.Count == 0)
                    return 0;

                // Count actual weeks with duties
                var dates = DutyShifts.Select(d => d.StartDate.Date).Distinct().OrderBy(d => d).ToList();
                if (dates.Count == 0)
                    return 0;

                DateTime firstDate = dates.First();
                DateTime lastDate = dates.Last();

                // Calculate number of weeks between first and last duty
                double weeks = Math.Max(1, (lastDate - firstDate).TotalDays / 7);

                // Calculate total hours
                double totalHours = DutyShifts.Sum(d => d.DurationHours);

                // Return weekly average
                return totalHours / weeks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating weekly average");
                return 0;
            }
        }

        private void UpdateTotalHours()
        {
            try
            {
                double totalHours = DutyShifts?.Sum(d => d.DurationHours) ?? 0;
                TotalHoursLabel = $"{totalHours:F1}/{_totalRequiredHours:F0} godzin";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating total hours");
                TotalHoursLabel = $"0.0/{_totalRequiredHours:F0} godzin";
            }
        }

        private void GroupAndDisplayDutyShifts()
        {
            try
            {
                // Group duty shifts by month and year
                var dutyShiftsByMonth = DutyShifts
                    .OrderByDescending(d => d.StartDate)
                    .GroupBy(d => new { Year = d.StartDate.Year, Month = d.StartDate.Month })
                    .ToList();

                GroupedDutyShifts.Clear();

                foreach (var monthGroup in dutyShiftsByMonth)
                {
                    var monthName = GetMonthName(monthGroup.Key.Month);
                    var year = monthGroup.Key.Year;
                    var title = $"{monthName} {year}";
                    var totalHours = monthGroup.Sum(d => d.DurationHours);
                    var subtitle = $"Łącznie: {totalHours:F1} godzin";

                    var groupedDutyShifts = new GroupedDutyShifts(title, subtitle, new ObservableCollection<DutyShift>(monthGroup));
                    GroupedDutyShifts.Add(groupedDutyShifts);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error grouping duty shifts");
            }
        }

        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Styczeń",
                2 => "Luty",
                3 => "Marzec",
                4 => "Kwiecień",
                5 => "Maj",
                6 => "Czerwiec",
                7 => "Lipiec",
                8 => "Sierpień",
                9 => "Wrzesień",
                10 => "Październik",
                11 => "Listopad",
                12 => "Grudzień",
                _ => "Nieznany"
            };
        }

        [RelayCommand]
        private async Task AddDutyShiftAsync()
        {
            await Shell.Current.Navigation.PushAsync(new DutyShiftDetailsPage(null, OnDutyShiftSaved));
        }

        private async Task OnDutyShiftSaved(DutyShift dutyShift)
        {
            try
            {
                // Save to database
                await _dutyShiftService.SaveDutyShiftAsync(dutyShift);

                // Refresh data
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving duty shift");
                throw;
            }
        }

        [RelayCommand]
        public async Task EditDutyShiftAsync(int dutyShiftId)
        {
            try
            {
                var dutyShift = await _dutyShiftService.GetDutyShiftAsync(dutyShiftId);
                if (dutyShift != null)
                {
                    await Shell.Current.Navigation.PushAsync(new DutyShiftDetailsPage(dutyShift, OnDutyShiftSaved));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to edit duty shift page");
                throw;
            }
        }
    }

    public class GroupedDutyShifts : ObservableCollection<DutyShift>
    {
        public string Title { get; private set; }
        public string Subtitle { get; private set; }

        public GroupedDutyShifts(string title, string subtitle, ObservableCollection<DutyShift> items) : base(items)
        {
            Title = title;
            Subtitle = subtitle;
        }
    }
}