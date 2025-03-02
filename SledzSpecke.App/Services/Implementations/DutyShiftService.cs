using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class DutyShiftService : IDutyShiftService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<DutyShiftService> _logger;

        public DutyShiftService(
            IDatabaseService databaseService,
            ILogger<DutyShiftService> logger)
        {
            this._databaseService = databaseService;
            this._logger = logger;
        }

        public async Task<List<DutyShift>> GetAllDutyShiftsAsync()
        {
            try
            {
                var userSettings = await this._databaseService.GetUserSettingsAsync();
                return await this._databaseService.QueryAsync<DutyShift>("SELECT * FROM DutyShifts WHERE SpecializationId = ? ORDER BY StartDate DESC", userSettings.CurrentSpecializationId);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting duty shifts");
                return new List<DutyShift>();
            }
        }

        public async Task<DutyShift> GetDutyShiftAsync(int id)
        {
            try
            {
                return await this._databaseService.GetByIdAsync<DutyShift>(id);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting duty shift with ID {Id}", id);
                return null;
            }
        }

        public async Task SaveDutyShiftAsync(DutyShift dutyShift)
        {
            try
            {
                var userSettings = await this._databaseService.GetUserSettingsAsync();
                dutyShift.SpecializationId = userSettings.CurrentSpecializationId;

                this._logger.LogDebug("Saving duty shift with ID: {Id} (0 means new record)", dutyShift.Id);

                if (dutyShift.Id == 0)
                {
                    // Insert new record approach
                    await this._databaseService.InsertAsync(dutyShift);
                    this._logger.LogInformation("New duty shift inserted with ID: {Id}", dutyShift.Id);
                }
                else
                {
                    // Update existing record
                    await this._databaseService.UpdateAsync(dutyShift);
                    this._logger.LogInformation("Existing duty shift updated with ID: {Id}", dutyShift.Id);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving duty shift with ID: {Id}", dutyShift.Id);
                throw;
            }
        }

        public async Task DeleteDutyShiftAsync(DutyShift dutyShift)
        {
            try
            {
                await this._databaseService.DeleteAsync(dutyShift);
                this._logger.LogInformation("Duty shift deleted successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error deleting duty shift");
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
                this._logger.LogError(ex, "Error calculating total duty hours");
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
                this._logger.LogError(ex, "Error calculating monthly duty hours");
                return new Dictionary<string, double>();
            }
        }

        public async Task<double> GetAverageWeeklyHoursAsync(int weeks = 4)
        {
            try
            {
                var dutyShifts = await this.GetAllDutyShiftsAsync();

                if (!dutyShifts.Any())
                    return 0;

                // Find the first and last duty dates
                DateTime firstDate = dutyShifts.Min(d => d.StartDate.Date);
                DateTime lastDate = dutyShifts.Max(d => d.StartDate.Date);

                // Calculate number of weeks between first and last duty
                double totalWeeks = Math.Max(1, (lastDate - firstDate).TotalDays / 7);

                // If less than requested weeks, use actual number
                totalWeeks = Math.Min(totalWeeks, weeks);

                // Calculate total hours
                double totalHours = dutyShifts.Sum(d => d.DurationHours);

                // Return weekly average
                return totalHours / totalWeeks;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error calculating average weekly hours");
                return 0;
            }
        }
    }
}