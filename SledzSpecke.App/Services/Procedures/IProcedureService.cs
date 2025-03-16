using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Procedures
{
    public interface IProcedureService
    {
        // Metody ogólne
        Task<List<ProcedureRequirement>> GetAvailableProcedureRequirementsAsync(int? moduleId = null);
        Task<ProcedureSummary> GetProcedureSummaryAsync(int? moduleId = null, int? procedureRequirementId = null);
        Task<(int completed, int total)> GetProcedureStatisticsForModuleAsync(int moduleId);

        // Metody dla starej wersji SMK
        Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null, int? year = null, int? requirementId = null);
        Task<RealizedProcedureOldSMK> GetOldSMKProcedureAsync(int procedureId);
        Task<bool> SaveOldSMKProcedureAsync(RealizedProcedureOldSMK procedure);
        Task<bool> DeleteOldSMKProcedureAsync(int procedureId);

        // Metody dla nowej wersji SMK
        Task<List<RealizedProcedureNewSMK>> GetNewSMKProceduresAsync(int? moduleId = null, int? procedureRequirementId = null);
        Task<RealizedProcedureNewSMK> GetNewSMKProcedureAsync(int procedureId);
        Task<bool> SaveNewSMKProcedureAsync(RealizedProcedureNewSMK procedure);
        Task<bool> DeleteNewSMKProcedureAsync(int procedureId);
    }
}