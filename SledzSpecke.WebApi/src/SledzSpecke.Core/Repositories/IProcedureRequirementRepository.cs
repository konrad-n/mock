using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IProcedureRequirementRepository
{
    Task<ProcedureRequirement?> GetByIdAsync(ProcedureRequirementId id);
    Task<ProcedureRequirement?> GetByCodeAsync(ModuleId moduleId, string code);
    Task<IEnumerable<ProcedureRequirement>> GetByModuleIdAsync(ModuleId moduleId);
    Task<IEnumerable<ProcedureRequirement>> GetBySpecializationIdAsync(int specializationId);
    Task<ProcedureRequirementId> AddAsync(ProcedureRequirement requirement);
    Task UpdateAsync(ProcedureRequirement requirement);
    Task DeleteAsync(ProcedureRequirement requirement);
    Task<bool> ExistsAsync(ModuleId moduleId, string code);
}