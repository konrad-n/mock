using SledzSpecke.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface IDutyRepository : IBaseRepository<Duty>
    {
        Task<List<Duty>> GetUserDutiesInRangeAsync(int userId, DateTime start, DateTime end);
        Task<decimal> GetTotalHoursAsync(int userId);
    }
}
