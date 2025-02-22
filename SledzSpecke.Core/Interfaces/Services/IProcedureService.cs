using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IProcedureService
    {
        Task<List<ProcedureExecution>> GetUserProceduresAsync();
        Task<ProcedureExecution> GetProcedureAsync(int id);
        Task<ProcedureExecution> AddProcedureAsync(ProcedureExecution procedure);
        Task<bool> UpdateProcedureAsync(ProcedureExecution procedure);
        Task<bool> DeleteProcedureAsync(int id);
        Task<Dictionary<string, int>> GetProcedureStatisticsAsync();
    }
}
