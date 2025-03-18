using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Procedures
{
    public interface IProcedureService
    {
        Task<List<ProcedureRequirement>> GetAvailableProcedureRequirementsAsync(int? moduleId = null);
        Task<ProcedureSummary> GetProcedureSummaryAsync(int? moduleId = null, int? procedureRequirementId = null);
        Task<(int completed, int total)> GetProcedureStatisticsForModuleAsync(int moduleId);
        Task<List<RealizedProcedureOldSMK>> GetOldSMKProceduresAsync(int? moduleId = null, int? year = null, int? requirementId = null);
        Task<RealizedProcedureOldSMK> GetOldSMKProcedureAsync(int procedureId);
        Task<bool> SaveOldSMKProcedureAsync(RealizedProcedureOldSMK procedure);
        Task<bool> DeleteOldSMKProcedureAsync(int procedureId);
        Task<List<RealizedProcedureNewSMK>> GetNewSMKProceduresAsync(int? moduleId = null, int? procedureRequirementId = null);
        Task<RealizedProcedureNewSMK> GetNewSMKProcedureAsync(int procedureId);
        Task<bool> SaveNewSMKProcedureAsync(RealizedProcedureNewSMK procedure);
        Task<bool> DeleteNewSMKProcedureAsync(int procedureId);
    }
}