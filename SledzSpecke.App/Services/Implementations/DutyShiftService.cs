using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class DutyShiftService : IDutyShiftService
    {
        private readonly IDatabaseService databaseService;
        private readonly ILogger<DutyShiftService> logger;

        public DutyShiftService(
            IDatabaseService databaseService,
            ILogger<DutyShiftService> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
        }

        public async Task<List<DutyShift>> GetAllDutyShiftsAsync()
        {
            try
            {
                var userSettings = await this.databaseService.GetUserSettingsAsync();
                return await this.databaseService.QueryAsync<DutyShift>("SELECT * FROM DutyShifts WHERE SpecializationId = ? ORDER BY StartDate DESC", userSettings.CurrentSpecializationId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting duty shifts");
                return new List<DutyShift>();
            }
        }

        public async Task<DutyShift> GetDutyShiftAsync(int id)
        {
            try
            {
                return await this.databaseService.GetByIdAsync<DutyShift>(id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting duty shift with ID {Id}", id);
                return null;
            }
        }

        public async Task SaveDutyShiftAsync(DutyShift dutyShift)
        {
            try
            {
                var userSettings = await this.databaseService.GetUserSettingsAsync();
                dutyShift.SpecializationId = userSettings.CurrentSpecializationId;

                this.logger.LogDebug("Saving duty shift with ID: {Id} (0 means new record)", dutyShift.Id);

                if (dutyShift.Id == 0)
                {
                    await this.databaseService.InsertAsync(dutyShift);
                    this.logger.LogInformation("New duty shift inserted with ID: {Id}", dutyShift.Id);
                }
                else
                {
                    await this.databaseService.UpdateAsync(dutyShift);
                    this.logger.LogInformation("Existing duty shift updated with ID: {Id}", dutyShift.Id);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving duty shift with ID: {Id}", dutyShift.Id);
                throw;
            }
        }

        public async Task DeleteDutyShiftAsync(DutyShift dutyShift)
        {
            try
            {
                await this.databaseService.DeleteAsync(dutyShift);
                this.logger.LogInformation("Duty shift deleted successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error deleting duty shift");
                throw;
            }
        }

        public async Task<double> GetTotalDutyHoursAsync()
        {
            try
            {
                var dutyShifts = await this.GetAllDutyShiftsAsync();
                return dutyShifts.Sum(d => d.DurationHours);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error calculating total duty hours");
                return 0;
            }
        }

        public async Task<Dictionary<string, double>> GetMonthlyDutyHoursAsync()
        {
            try
            {
                var dutyShifts = await this.GetAllDutyShiftsAsync();
                return dutyShifts
                    .GroupBy(d => new { d.StartDate.Year, d.StartDate.Month })
                    .OrderByDescending(g => g.Key.Year)
                    .ThenByDescending(g => g.Key.Month)
                    .ToDictionary(
                        g => $"{g.Key.Year}-{g.Key.Month:00}",
                        g => g.Sum(d => d.DurationHours)
                    );
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error calculating monthly duty hours");
                return new Dictionary<string, double>();
            }
        }

        public async Task<double> GetAverageWeeklyHoursAsync(int weeks = 4)
        {
            try
            {
                var dutyShifts = await this.GetAllDutyShiftsAsync();

                if (!dutyShifts.Any())
                {
                    return 0;
                }
                DateTime firstDate = dutyShifts.Min(d => d.StartDate.Date);
                DateTime lastDate = dutyShifts.Max(d => d.StartDate.Date);
                double totalWeeks = Math.Max(1, (lastDate - firstDate).TotalDays / 7);
                totalWeeks = Math.Min(totalWeeks, weeks);
                double totalHours = dutyShifts.Sum(d => d.DurationHours);
                return totalHours / totalWeeks;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error calculating average weekly hours");
                return 0;
            }
        }
    }
}
