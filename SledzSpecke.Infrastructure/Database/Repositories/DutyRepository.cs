using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public class DutyRepository : BaseRepository<Duty>, IDutyRepository
    {
        public DutyRepository(IApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Duty>> GetUserDutiesInRangeAsync(int userId, DateTime start, DateTime end)
        {
            await _context.InitializeAsync();
            return await _connection.Table<Duty>()
                .Where(d => d.UserId == userId
                        && d.StartTime >= start
                        && d.EndTime <= end)
                .OrderByDescending(d => d.StartTime)
                .ToListAsync();
        }

        public async Task<Dictionary<DateTime, decimal>> GetMonthlyHoursAsync(int userId, DateTime startDate, DateTime endDate)
        {
            await _context.InitializeAsync();
            var duties = await GetUserDutiesInRangeAsync(userId, startDate, endDate);

            return duties
                .GroupBy(d => new DateTime(d.StartTime.Year, d.StartTime.Month, 1))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(d => (decimal)(d.EndTime - d.StartTime).TotalHours)
                );
        }

        public async Task<bool> HasOverlappingDutyAsync(int userId, DateTime start, DateTime end, int? excludeDutyId = null)
        {
            await _context.InitializeAsync();
            var query = _connection.Table<Duty>()
                .Where(d => d.UserId == userId);

            if (excludeDutyId.HasValue)
            {
                query = query.Where(d => d.Id != excludeDutyId.Value);
            }

            return await query.Where(d =>
                (d.StartTime <= start && d.EndTime > start) ||
                (d.StartTime < end && d.EndTime >= end) ||
                (d.StartTime >= start && d.EndTime <= end)
            ).CountAsync() > 0;
        }

        public async Task<DutyStatistics> GetDutyStatisticsAsync(int userId)
        {
            await _context.InitializeAsync();
            var duties = await _connection.Table<Duty>()
                .Where(d => d.UserId == userId)
                .ToListAsync();

            var statistics = new DutyStatistics
            {
                TotalHours = duties.Sum(d => (decimal)(d.EndTime - d.StartTime).TotalHours),
                MonthlyHours = duties
                    .Where(d => d.StartTime.Month == DateTime.Today.Month && d.StartTime.Year == DateTime.Today.Year)
                    .Sum(d => (decimal)(d.EndTime - d.StartTime).TotalHours),
                TotalCount = duties.Count,
                DutiesByType = duties
                    .GroupBy(d => d.Type)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return statistics;
        }
    }
}
