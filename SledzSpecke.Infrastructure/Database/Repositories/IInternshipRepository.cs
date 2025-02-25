using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface IProcedureRepository : IBaseRepository<ProcedureExecution>
    {
        // Istniejące metody
        Task<List<ProcedureExecution>> GetUserProceduresAsync(int userId);
        Task<Dictionary<string, int>> GetProcedureStatsAsync(int userId);
        Task<ProcedureExecution> GetProcedureWithDetailsAsync(int id);
        Task<List<ProcedureDefinition>> SearchAsync(string query);
        
        // Nowe metody
        Task<List<ProcedureRequirement>> GetRequirementsForSpecializationAsync(int specializationId);
        Task<List<ProcedureRequirement>> GetRequirementsByStageAsync(int specializationId, string stage);
        Task<List<ProcedureRequirement>> GetRequirementsByCategoryAsync(int specializationId, string category);
        Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByCategoryAsync(int userId, int specializationId);
        Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByStageAsync(int userId, int specializationId);
    }
}
