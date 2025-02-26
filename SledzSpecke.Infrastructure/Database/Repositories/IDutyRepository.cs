using SledzSpecke.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface IDutyRepository : IBaseRepository<Duty>
    {
        Task<List<Duty>> GetUserDutiesInRangeAsync(int userId, DateTime start, DateTime end);
        Task<Dictionary<DateTime, decimal>> GetMonthlyHoursAsync(int userId, DateTime startDate, DateTime endDate);
        Task<bool> HasOverlappingDutyAsync(int userId, DateTime start, DateTime end, int? excludeDutyId = null);
        Task<DutyStatistics> GetDutyStatisticsAsync(int userId);
    }
}
