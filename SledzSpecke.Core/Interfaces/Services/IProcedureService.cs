using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IProcedureService
    {
        // Istniejące metody
        Task<List<ProcedureExecution>> GetUserProceduresAsync();
        Task<ProcedureExecution> GetProcedureAsync(int id);
        Task<ProcedureExecution> AddProcedureAsync(ProcedureExecution procedure);
        Task<bool> UpdateProcedureAsync(ProcedureExecution procedure);
        Task<bool> DeleteProcedureAsync(int id);
        Task<List<ProcedureRequirement>> GetRequirementsForSpecializationAsync();
        Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByCategoryAsync();
        Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByStageAsync();
        Task<double> GetProcedureCompletionPercentageAsync();
        Task<List<string>> GetAvailableCategoriesAsync();
        Task<List<string>> GetAvailableStagesAsync();
        Task<bool> ValidateProcedureRequirementsAsync(ProcedureExecution procedure);
    }
}
