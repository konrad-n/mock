using SledzSpecke.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IDutyService
    {
        Task<List<Duty>> GetUserDutiesAsync(DateTime? fromDate = null);
        Task<Duty> GetDutyAsync(int id);
        Task<Duty> AddDutyAsync(Duty duty);
        Task<bool> UpdateDutyAsync(Duty duty);
        Task<bool> DeleteDutyAsync(int id);
        Task<DutyStatistics> GetDutyStatisticsAsync();
    }
}
