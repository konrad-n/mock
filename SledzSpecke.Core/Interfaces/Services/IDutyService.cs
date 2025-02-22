using SledzSpecke.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IDutyService
    {
        Task<List<Duty>> GetUserDutiesAsync(int userId, DateTime? fromDate = null);
        Task<Duty> AddDutyAsync(Duty duty);
        Task<DutyStatistics> GetDutyStatisticsAsync(int userId);
        Task<bool> IsDutyOverlappingAsync(int userId, DateTime start, DateTime end);
        Task<decimal> GetRemainingRequiredHoursAsync(int userId);
    }
}
