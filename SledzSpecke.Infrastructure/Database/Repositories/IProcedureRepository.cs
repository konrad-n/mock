using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface IProcedureRepository : IBaseRepository<ProcedureExecution>
    {
        Task<List<ProcedureExecution>> GetUserProceduresAsync(int userId);
        Task<Dictionary<string, int>> GetProcedureStatsAsync(int userId);
        Task<ProcedureExecution> GetProcedureWithDetailsAsync(int id);
        Task<List<ProcedureDefinition>> SearchAsync(string query);
    }

}
