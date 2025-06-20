using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IProcedureRealizationRepository
{
    Task<ProcedureRealization?> GetByIdAsync(ProcedureRealizationId id);
    Task<IEnumerable<ProcedureRealization>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<ProcedureRealization>> GetByRequirementIdAsync(ProcedureRequirementId requirementId);
    Task<IEnumerable<ProcedureRealization>> GetByUserAndRequirementAsync(UserId userId, ProcedureRequirementId requirementId);
    Task<IEnumerable<ProcedureRealization>> GetByUserAndModuleAsync(UserId userId, ModuleId moduleId);
    Task<ProcedureRealizationId> AddAsync(ProcedureRealization realization);
    Task UpdateAsync(ProcedureRealization realization);
    Task DeleteAsync(ProcedureRealization realization);
    Task<int> CountByUserAndRequirementAsync(UserId userId, ProcedureRequirementId requirementId, ProcedureRole? role = null);
    Task<Dictionary<ProcedureRequirementId, int>> CountByUserAndModuleGroupedAsync(UserId userId, ModuleId moduleId, ProcedureRole? role = null);
}