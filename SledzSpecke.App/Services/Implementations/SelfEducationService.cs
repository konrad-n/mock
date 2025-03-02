using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class SelfEducationService : ISelfEducationService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<SelfEducationService> _logger;

        public SelfEducationService(
            IDatabaseService databaseService,
            ILogger<SelfEducationService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<List<SelfEducation>> GetAllSelfEducationAsync()
        {
            try
            {
                var userSettings = await _databaseService.GetUserSettingsAsync();
                return await _databaseService.QueryAsync<SelfEducation>("SELECT * FROM SelfEducation WHERE SpecializationId = ? ORDER BY StartDate DESC", userSettings.CurrentSpecializationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting self-education activities");
                return new List<SelfEducation>();
            }
        }

        public async Task<SelfEducation> GetSelfEducationAsync(int id)
        {
            try
            {
                return await _databaseService.GetByIdAsync<SelfEducation>(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting self-education with ID {Id}", id);
                return null;
            }
        }

        public async Task SaveSelfEducationAsync(SelfEducation selfEducation)
        {
            try
            {
                var userSettings = await _databaseService.GetUserSettingsAsync();
                selfEducation.SpecializationId = userSettings.CurrentSpecializationId;

                await _databaseService.SaveAsync(selfEducation);
                _logger.LogInformation("Self-education saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving self-education");
                throw;
            }
        }

        public async Task DeleteSelfEducationAsync(SelfEducation selfEducation)
        {
            try
            {
                await _databaseService.DeleteAsync(selfEducation);
                _logger.LogInformation("Self-education deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting self-education");
                throw;
            }
        }

        public async Task<int> GetTotalUsedDaysAsync()
        {
            try
            {
                var activities = await GetAllSelfEducationAsync();
                return activities.Sum(s => s.DurationDays);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total self-education days");
                return 0;
            }
        }

        public async Task<Dictionary<int, int>> GetYearlyUsedDaysAsync()
        {
            try
            {
                var activities = await GetAllSelfEducationAsync();
                return activities
                    .GroupBy(s => s.StartDate.Year)
                    .OrderByDescending(g => g.Key)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(s => s.DurationDays)
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating yearly self-education days");
                return new Dictionary<int, int>();
            }
        }

        public async Task<int> GetYearlyAllowanceAsync()
        {
            try
            {
                var specialization = await _databaseService.GetCurrentSpecializationAsync();
                return specialization?.SelfEducationDaysPerYear ?? 6; // Default to 6 days if not specified
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting yearly self-education allowance");
                return 6; // Default to 6 days in case of error
            }
        }
    }
}