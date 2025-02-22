using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IProcedureService
    {
        Task<List<ProcedureDefinition>> GetRequiredProceduresAsync(int specializationId);
        Task<List<ProcedureExecution>> GetUserProceduresAsync(int userId);
        Task<ProcedureExecution> AddProcedureExecutionAsync(ProcedureExecution execution);
        Task<Dictionary<string, int>> GetProcedureStatisticsAsync(int userId);
    }
}
