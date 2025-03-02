using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.App.Features.Duties.Views;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Features.Duties.ViewModels
{
    public partial class DutyShiftsViewModel : ViewModelBase
    {
        private readonly IDutyShiftService dutyShiftService;
        private readonly ISpecializationService specializationService;
        private Specialization specialization;
        private double totalRequiredHours;

        [ObservableProperty]
        private ObservableCollection<DutyShift> dutyShifts;

        [ObservableProperty]
        private string totalHoursLabel;

        [ObservableProperty]
        private string weeklyHoursLabel;

        [ObservableProperty]
        private bool isNoDutyShiftsVisible;

        [ObservableProperty]
        private ObservableCollection<GroupedDutyShifts> groupedDutyShifts;

        public DutyShiftsViewModel(
            IDutyShiftService dutyShiftService,
            ISpecializationService specializationService,
            ILogger<DutyShiftsViewModel> logger)
            : base(logger)
        {
            this.dutyShiftService = dutyShiftService;
            this.specializationService = specializationService;
            this.DutyShifts = new ObservableCollection<DutyShift>();
            this.GroupedDutyShifts = new ObservableCollection<GroupedDutyShifts>();
            this.Title = "Dyzury";
        }

        [RelayCommand]
        public async Task EditDutyShiftAsync(int dutyShiftId)
        {
            try
            {
                var dutyShift = await this.dutyShiftService.GetDutyShiftAsync(dutyShiftId);
                if (dutyShift != null)
                {
                    await Shell.Current.Navigation.PushAsync(new DutyShiftDetailsPage(dutyShift, this.OnDutyShiftSaved));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error navigating to edit duty shift page");
                throw;
            }
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
                this.logger.LogError(ex, "Error loading duty shifts data");
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
                var allDutyShifts = await this.dutyShiftService.GetAllDutyShiftsAsync();
                this.DutyShifts = new ObservableCollection<DutyShift>(allDutyShifts);
                this.specialization = await this.specializationService.GetSpecializationAsync();
                this.totalRequiredHours = this.specialization.RequiredDutyHoursPerWeek * (this.specialization.BaseDurationWeeks / 52.0) * 52;
                double weeklyAverage = this.CalculateWeeklyAverage();
                this.UpdateTotalHours();
                this.GroupAndDisplayDutyShifts();
                this.WeeklyHoursLabel = $"{weeklyAverage:F1}h";
                this.IsNoDutyShiftsVisible = this.DutyShifts.Count == 0;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading duty shifts");
                throw;
            }
        }

        private double CalculateWeeklyAverage()
        {
            try
            {
                if (this.DutyShifts == null || this.DutyShifts.Count == 0)
                {
                    return 0;
                }

                var dates = this.DutyShifts.Select(d => d.StartDate.Date).Distinct().OrderBy(d => d).ToList();
                if (dates.Count == 0)
                {
                    return 0;
                }

                DateTime firstDate = dates.First();
                DateTime lastDate = dates.Last();
                double weeks = Math.Max(1, (lastDate - firstDate).TotalDays / 7);
                double totalHours = this.DutyShifts.Sum(d => d.DurationHours);
                return totalHours / weeks;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error calculating weekly average");
                return 0;
            }
        }

        private void UpdateTotalHours()
        {
            try
            {
                double totalHours = this.DutyShifts?.Sum(d => d.DurationHours) ?? 0;
                this.TotalHoursLabel = $"{totalHours:F1}/{this.totalRequiredHours:F0} godzin";
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error updating total hours");
                this.TotalHoursLabel = $"0.0/{this.totalRequiredHours:F0} godzin";
            }
        }

        private void GroupAndDisplayDutyShifts()
        {
            try
            {
                var dutyShiftsByMonth = this.DutyShifts
                    .OrderByDescending(d => d.StartDate)
                    .GroupBy(d => new { Year = d.StartDate.Year, Month = d.StartDate.Month })
                    .ToList();
                this.GroupedDutyShifts.Clear();
                foreach (var monthGroup in dutyShiftsByMonth)
                {
                    var monthName = this.GetMonthName(monthGroup.Key.Month);
                    var year = monthGroup.Key.Year;
                    var title = $"{monthName} {year}";
                    var totalHours = monthGroup.Sum(d => d.DurationHours);
                    var subtitle = $"Lacznie: {totalHours:F1} godzin";

                    var groupedDutyShiftsWithMonthGroup = new GroupedDutyShifts(title, subtitle, new ObservableCollection<DutyShift>(monthGroup));
                    this.GroupedDutyShifts.Add(groupedDutyShiftsWithMonthGroup);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error grouping duty shifts");
            }
        }

        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Styczen",
                2 => "Luty",
                3 => "Marzec",
                4 => "Kwiecien",
                5 => "Maj",
                6 => "Czerwiec",
                7 => "Lipiec",
                8 => "Sierpien",
                9 => "Wrzesien",
                10 => "Pazdziernik",
                11 => "Listopad",
                12 => "Grudzien",
                _ => "Nieznany"
            };
        }

        [RelayCommand]
        private async Task AddDutyShiftAsync()
        {
            await Shell.Current.Navigation.PushAsync(new DutyShiftDetailsPage(null, this.OnDutyShiftSaved));
        }

        private async Task OnDutyShiftSaved(DutyShift dutyShift)
        {
            try
            {
                await this.dutyShiftService.SaveDutyShiftAsync(dutyShift);
                await this.LoadDataAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving duty shift");
                throw;
            }
        }
    }
}
