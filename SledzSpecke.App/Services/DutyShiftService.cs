using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services
{
    public class DutyShiftService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<DutyShiftService> _logger;

        public DutyShiftService(DatabaseService databaseService, ILogger<DutyShiftService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<List<DutyShift>> GetAllDutyShiftsAsync()
        {
            try
            {
                var userSettings = await _databaseService.GetUserSettingsAsync();
                return await _databaseService.QueryAsync<DutyShift>("SELECT * FROM DutyShifts WHERE SpecializationId = ? ORDER BY StartDate DESC", userSettings.CurrentSpecializationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting duty shifts");
                return new List<DutyShift>();
            }
        }

        public async Task<DutyShift> GetDutyShiftAsync(int id)
        {
            try
            {
                return await _databaseService.GetByIdAsync<DutyShift>(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting duty shift with ID {Id}", id);
                return null;
            }
        }

        public async Task SaveDutyShiftAsync(DutyShift dutyShift)
        {
            try
            {
                var userSettings = await _databaseService.GetUserSettingsAsync();
                dutyShift.SpecializationId = userSettings.CurrentSpecializationId;

                await _databaseService.SaveAsync(dutyShift);
                _logger.LogInformation("Duty shift saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving duty shift");
                throw;
            }
        }

        public async Task DeleteDutyShiftAsync(DutyShift dutyShift)
        {
            try
            {
                await _databaseService.DeleteAsync(dutyShift);
                _logger.LogInformation("Duty shift deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting duty shift");
                throw;
            }
        }

        public async Task<double> GetTotalDutyHoursAsync()
        {
            try
            {
                var dutyShifts = await GetAllDutyShiftsAsync();
                return dutyShifts.Sum(d => d.DurationHours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total duty hours");
                return 0;
            }
        }

        public async Task<Dictionary<string, double>> GetMonthlyDutyHoursAsync()
        {
            try
            {
                var dutyShifts = await GetAllDutyShiftsAsync();
                return dutyShifts
                    .GroupBy(d => new { Year = d.StartDate.Year, Month = d.StartDate.Month })
                    .OrderByDescending(g => g.Key.Year)
                    .ThenByDescending(g => g.Key.Month)
                    .ToDictionary(
                        g => $"{g.Key.Year}-{g.Key.Month:00}",
                        g => g.Sum(d => d.DurationHours)
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating monthly duty hours");
                return new Dictionary<string, double>();
            }
        }

        public async Task<double> GetAverageWeeklyHoursAsync(int weeks = 4)
        {
            try
            {
                var dutyShifts = await GetAllDutyShiftsAsync();

                // Calculate hours in last N weeks
                var cutoffDate = DateTime.Now.AddDays(-7 * weeks);
                var recentShifts = dutyShifts.Where(d => d.StartDate >= cutoffDate);

                if (!recentShifts.Any())
                    return 0;

                var totalHours = recentShifts.Sum(d => d.DurationHours);
                return totalHours / weeks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average weekly hours");
                return 0;
            }
        }
    }
}