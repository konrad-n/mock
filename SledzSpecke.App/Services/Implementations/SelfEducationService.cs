using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class SelfEducationService : ISelfEducationService
    {
        private readonly IDatabaseService databaseService;
        private readonly ILogger<SelfEducationService> logger;

        public SelfEducationService(
            IDatabaseService databaseService,
            ILogger<SelfEducationService> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
        }

        public async Task<List<SelfEducation>> GetAllSelfEducationAsync()
        {
            try
            {
                var userSettings = await this.databaseService.GetUserSettingsAsync();
                return await this.databaseService.QueryAsync<SelfEducation>("SELECT * FROM SelfEducation WHERE SpecializationId = ? ORDER BY StartDate DESC", userSettings.CurrentSpecializationId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting self-education activities");
                return new List<SelfEducation>();
            }
        }

        public async Task<SelfEducation> GetSelfEducationAsync(int id)
        {
            try
            {
                return await this.databaseService.GetByIdAsync<SelfEducation>(id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting self-education with ID {Id}", id);
                return null;
            }
        }

        public async Task SaveSelfEducationAsync(SelfEducation selfEducation)
        {
            try
            {
                var userSettings = await this.databaseService.GetUserSettingsAsync();
                selfEducation.SpecializationId = userSettings.CurrentSpecializationId;

                await this.databaseService.SaveAsync(selfEducation);
                this.logger.LogInformation("Self-education saved successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving self-education");
                throw;
            }
        }

        public async Task DeleteSelfEducationAsync(SelfEducation selfEducation)
        {
            try
            {
                await this.databaseService.DeleteAsync(selfEducation);
                this.logger.LogInformation("Self-education deleted successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error deleting self-education");
                throw;
            }
        }

        public async Task<int> GetTotalUsedDaysAsync()
        {
            try
            {
                var activities = await this.GetAllSelfEducationAsync();
                return activities.Sum(s => s.DurationDays);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error calculating total self-education days");
                return 0;
            }
        }

        public async Task<Dictionary<int, int>> GetYearlyUsedDaysAsync()
        {
            try
            {
                var activities = await this.GetAllSelfEducationAsync();
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
                this.logger.LogError(ex, "Error calculating yearly self-education days");
                return new Dictionary<int, int>();
            }
        }

        public async Task<int> GetYearlyAllowanceAsync()
        {
            try
            {
                var specialization = await this.databaseService.GetCurrentSpecializationAsync();
                return specialization?.SelfEducationDaysPerYear ?? 6;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting yearly self-education allowance");
                return 6;
            }
        }
    }
}